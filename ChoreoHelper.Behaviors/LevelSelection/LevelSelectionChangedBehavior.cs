using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.LevelSelection;

public sealed class LevelSelectionChangedBehavior : IBehavior<LevelSelectionViewModel>
{
    public void Activate(LevelSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                MessageBus.Current.SendMessage(new LevelChanged(vm.Level, vm.IsSelected));
            })
            .DisposeWith(disposables);
    }
}