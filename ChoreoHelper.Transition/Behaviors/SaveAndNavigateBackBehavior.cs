using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using Microsoft.Extensions.Logging;

namespace ChoreoHelper.Transition.Behaviors;

public sealed class SaveAndNavigateBackBehavior(
    DancesCache dancesCache,
    IPublisher<CloseEditTransition> closeEditTransitionPublisher,
    ILogger<SaveAndNavigateBackBehavior> logger)
    : IBehavior<TransitionViewModel>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Activate(TransitionViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default, CanExecute(viewModel));

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

    private IObservable<bool> CanExecute(TransitionViewModel viewModel)
    {
        return viewModel
            .WhenAnyValue(
                vm => vm.SelectedDistance,
                vm => vm.SelectedRestriction)
            .Select(_ => viewModel)
            .Select(vm =>
                _semaphore.CurrentCount > 0
                && !string.IsNullOrEmpty(vm.FromFigureName)
                && !string.IsNullOrEmpty(vm.ToFigureName)
                && vm.SelectedDistance is not null
                && vm.SelectedRestriction is not null);
    }

    private void SaveChangesWithLock(TransitionViewModel viewModel)
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

    private void SaveChanges(TransitionViewModel viewModel)
    {
        if (viewModel.SelectedDistance is not {} selectedDistance
            || viewModel.SelectedRestriction is not {} selectedRestriction)
        {
            logger.LogWarning("Selected distance or restriction was null");
            return;
        }

        var dance = dancesCache.Items.FirstOrDefault(d => d.Name == viewModel.DanceName);
        if (dance is null)
        {
            logger.LogWarning("Dance {DanceName} was not found", viewModel.DanceName);
            return;
        }

        var fromFigure = dance.Figures.FirstOrDefault(f => f.Name == viewModel.FromFigureName);
        if (fromFigure is null)
        {
            logger.LogWarning("Figure {FromFigureName} was not found", viewModel.FromFigureName);
            return;
        }

        var toFigure = dance.Figures.FirstOrDefault(f => f.Name == viewModel.ToFigureName);
        if (toFigure is null)
        {
            logger.LogWarning("Figure {ToFigureName} was not found", viewModel.ToFigureName);
            return;
        }

        // get the figure from the view model
        var transitionFromCache =
            (from transition in dance.Transitions
            where transition.Source == fromFigure
            where transition.Target == toFigure
            select transition)
            .FirstOrDefault();

        if (transitionFromCache is null)
        {
            logger.LogWarning("Transition from {FromFigureName} to {ToFigureName} was not found", viewModel.FromFigureName, viewModel.ToFigureName);
            return;
        }

        var newTransition = new DanceFigureTransition(
            transitionFromCache.Source,
            transitionFromCache.Target,
            selectedDistance.Distance,
            selectedRestriction.Restriction);

        // update cache
        dance.Transitions.Remove(transitionFromCache);
        dance.Transitions.Add(newTransition);
        dancesCache.Refresh();
    }

    private void NavigateBack()
    {
        closeEditTransitionPublisher.Publish(new());
    }
}