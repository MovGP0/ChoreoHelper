using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class CloseDrawerBehavior: IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        MessageBus.Current
            .Listen<CloseDrawer>()
            .Subscribe(_ => viewModel.IsDrawerOpen = false)
            .DisposeWith(disposables);
    }
}