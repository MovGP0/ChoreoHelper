using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using Microsoft.Extensions.Logging;

namespace ChoreoHelper.CreateFigure.Behaviors;

public sealed class SaveAndNavigateBackBehavior(
    DancesCache dancesCache,
    IPublisher<CloseEditFigure> closeEditFigurePublisher,
    ILogger<SaveAndNavigateBackBehavior> logger)
    : IBehavior<CreateFigureViewModel>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Activate(CreateFigureViewModel viewModel, CompositeDisposable disposables)
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

    private IObservable<bool> CanExecute(CreateFigureViewModel viewModel)
    {
        return viewModel
            .WhenAnyValue(
                vm => vm.Name,
                vm => vm.Level,
                vm => vm.Restriction)
            .Select(_ => viewModel)
            .Select(vm =>
                _semaphore.CurrentCount > 0
                && vm.Level is not null
                && vm.Restriction is not null
                && IsValidDanceFigureName(vm.DanceName, vm.Name));
    }

    private bool IsValidDanceFigureName(string danceName, string figureName)
    {
        if (string.IsNullOrWhiteSpace(danceName)
            || string.IsNullOrWhiteSpace(figureName))
        {
            return false;
        }

        var dance = dancesCache.Items.FirstOrDefault(d => d.Name == danceName);
        return dance is not null
            && dance.Figures.All(f => f.Name != figureName);
    }

    private void SaveChangesWithLock(CreateFigureViewModel viewModel)
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

    private void SaveChanges(CreateFigureViewModel viewModel)
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

        // Check if a figure with the same name already exists
        var existingFigure = dance.Figures.FirstOrDefault(f => f.Name == viewModel.Name);
        if (existingFigure is not null)
        {
            logger.LogWarning("Figure with name {Name} already exists in dance {DanceName}", viewModel.Name, viewModel.DanceName);
            return;
        }

        // Create and add the new figure
        var newFigure = new DanceFigure(dance, viewModel.Name, level.Level, restriction.Restriction);
        dance.Figures.Add(newFigure);
        
        logger.LogInformation("Created new figure {Name} in dance {DanceName}", viewModel.Name, viewModel.DanceName);

        dancesCache.Refresh();
    }

    private void NavigateBack()
    {
        closeEditFigurePublisher.Publish(new());
    }
}