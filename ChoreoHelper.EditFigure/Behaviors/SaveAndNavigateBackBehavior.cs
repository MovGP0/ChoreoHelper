using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using Microsoft.Extensions.Logging;

namespace ChoreoHelper.EditFigure.Behaviors;

public sealed class SaveAndNavigateBackBehavior(
    DancesCache dancesCache,
    IPublisher<CloseEditFigure> closeEditFigurePublisher,
    ILogger<SaveAndNavigateBackBehavior> logger) : IBehavior<EditFigureViewModel>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Activate(EditFigureViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default, CanExecute(viewModel)).DisposeWith(disposables);

        viewModel.SaveAndNavigateBack = command;
        Disposable.Create(() => viewModel.NavigateBack = DisabledCommand.Instance).DisposeWith(disposables);

        command
            .Subscribe(_ =>
            {
                SaveChangesWithLock(viewModel);
                NavigateBack();
            })
            .DisposeWith(disposables);
    }

    private IObservable<bool> CanExecute(EditFigureViewModel viewModel)
    {
        return viewModel
            .WhenAnyValue(
                vm => vm.Name,
                vm => vm.Level,
                vm => vm.Restriction)
            .Select(_ => viewModel)
            .Select(vm =>
                _semaphore.CurrentCount > 0
                && !string.IsNullOrEmpty(vm.Name)
                && vm.Level is not null
                && vm.Restriction is not null);
    }

    private void SaveChangesWithLock(EditFigureViewModel viewModel)
    {
        if (!_semaphore.Wait(TimeSpan.FromMilliseconds(300)))
        {
            logger.LogWarning("Could not acquire semaphore");
            return;
        }
        try
        {
            SaveChanges(viewModel);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void SaveChanges(EditFigureViewModel viewModel)
    {
        if (string.IsNullOrEmpty(viewModel.Name))
        {
            logger.LogWarning("Figure name was null or empty");
            return;
        }
        
        if (viewModel.Level is not {} level)
        {
            logger.LogWarning("Level was null");
            return;
        }

        if (viewModel.Restriction is not {} restriction)
        {
            logger.LogWarning("Restriction was null");
            return;
        }

        var dance = dancesCache.Items.FirstOrDefault(d => d.Name == viewModel.DanceName);
        if (dance is null)
        {
            logger.LogWarning("Dance {DanceName} was not found", viewModel.DanceName);
            return;
        }

        var oldFigure = dance.Figures.FirstOrDefault(f => f.Hash == viewModel.Hash);
        if (oldFigure is null)
        {
            logger.LogWarning("Figure {Name} was not found", viewModel.Name);
            return;
        }

        var newFigure = new DanceFigure(dance, viewModel.Name, level.Level, restriction.Restriction);

        var transitionsToRecreate =
            (from transition in dance.Transitions
            where transition.Source == oldFigure
                  || transition.Target == oldFigure
            select transition)
            .ToImmutableArray();

        var newTransitions =
            from oldTransition in transitionsToRecreate
            let source = oldTransition.Source == oldFigure ? newFigure : oldTransition.Source
            let target = oldTransition.Target == oldFigure ? newFigure : oldTransition.Target
            select new DanceFigureTransition(source, target, oldTransition.Distance, oldTransition.Restriction);

        dance.Figures.Remove(oldFigure);
        dance.Figures.Add(newFigure);
        dance.Transitions.RemoveMany(transitionsToRecreate);
        dance.Transitions.AddRange(newTransitions);

        dancesCache.Refresh();
    }

    private void NavigateBack()
    {
        closeEditFigurePublisher.Publish(new());
    }
}