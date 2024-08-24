using ChoreoHelper.Behaviors.Extensions;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadFiguresBehavior(IDanceFiguresRepository connection) : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var figures = new SourceCache<FigureViewModel, string>(vm => vm.Hash)
            .DisposeWith(disposables);

        figures.Connect()
            .Bind(viewModel.Figures)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
            .Select(_ => viewModel)
            .Select(vm =>
            {
                if (vm.SelectedDance is null)
                {
                    return Array.Empty<FigureViewModel>();
                }

                return connection
                    .GetFigures(vm.SelectedDance.Name, vm.GetLevels())
                    .Select(ToViewModel)
                    .ToArray();
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Subscribe(fs =>
            {
                figures.Update(fs);
            })
            .DisposeWith(disposables);
    }

    private static IObservable<Unit> Observe(MainWindowViewModel viewModel)
    {
        var selectedDanceChanged = viewModel
            .WhenAnyValue(vm => vm.SelectedDance)
            .Select(_ => Unit.Default);
            
        var levelChanged = MessageBus.Current
            .Listen<LevelChanged>()
            .Select(_ => Unit.Default);

        return selectedDanceChanged.Merge(levelChanged);
    }

    [Pure]
    private static FigureViewModel ToViewModel(DanceStepNodeInfo loadedFigure)
    {
        var vm = Locator.Current.GetRequiredService<FigureViewModel>();
        vm.Hash = loadedFigure.Hash;
        vm.Name = loadedFigure.Name;
        vm.Level = loadedFigure.Level;
        return vm;
    }
}