using ChoreoHelper.Gateway;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadDancesBehavior(IDanceFiguresRepository connection) : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var dancesList = new SourceList<string>();

        dancesList.Connect()
            .Bind(viewModel.Dances)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        Observable
            .Return(true)
            .Select(_ => GetDancesInAlphabeticalOrder())
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Subscribe(items =>
            {
                dancesList.Clear();
                dancesList.AddRange(items);
            })
            .DisposeWith(disposables);
    }

    private string[] GetDancesInAlphabeticalOrder()
    {
        return connection
            .GetDances()
            .Order()
            .ToArray();
    }
}