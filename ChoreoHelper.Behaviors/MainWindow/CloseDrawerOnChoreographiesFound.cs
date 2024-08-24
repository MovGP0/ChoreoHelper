using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class CloseDrawerOnChoreographiesFound : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel.Choreographies
            .WhenAnyPropertyChanged()
            .SubscribeOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                vm.IsDrawerOpen = false;
            })
            .DisposeWith(disposables);
    }
}