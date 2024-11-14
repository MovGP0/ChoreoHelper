using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.LevelSelection;

public sealed class LevelSelectionChangedBehavior(
    IPublisher<LevelChanged> levelChangedPublisher)
    : IBehavior<LevelSelectionViewModel>
{
    public void Activate(LevelSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .Select(_ => viewModel)
            .Subscribe(vm => levelChangedPublisher.Publish(new LevelChanged(vm.Level, vm.IsSelected)))
            .DisposeWith(disposables);
    }
}