using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadRequiredFiguresBehavior(IDanceFiguresRepository connection) : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var requiredFigures = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Hash);

        requiredFigures
            .Connect()
            .Bind(viewModel.RequiredFigures)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
            .Select(_ => viewModel)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Select(vm => GetFiguresForDance(vm.SelectedDance, vm.GetLevels()))
            .Subscribe(loadedFigures =>
            {
                foreach (var loadedFigure in loadedFigures)
                {
                    var vm = ToViewModel(loadedFigure);
                    requiredFigures.AddOrUpdate(vm);
                }

                var toDelete = requiredFigures.Keys
                    .Except(loadedFigures.Select(e => e.Hash));

                requiredFigures.RemoveKeys(toDelete);
            })
            .DisposeWith(disposables);
    }

    private static IObservable<Unit> Observe(MainWindowViewModel viewModel)
    {
        var obs0 = viewModel
            .WhenAnyValue(vm => vm.SelectedDance)
            .Select(_ => Unit.Default);

        var obs1 = MessageBus.Current
            .Listen<LevelChanged>()
            .Select(_ => Unit.Default);

        return obs0.Merge(obs1);
    }

    [Pure]
    private static RequiredFigureSelectionViewModel ToViewModel(DanceStepNodeInfo loadedFigure)
    {
        var vm = Locator.Current.GetRequiredService<RequiredFigureSelectionViewModel>();
        vm.Hash = loadedFigure.Hash;
        vm.Name = loadedFigure.Name;
        vm.IsSelected = false;
        vm.Level = loadedFigure.Level;
        return vm;
    }

    [Pure]
    private DanceStepNodeInfo[] GetFiguresForDance(string? selectedDance, DanceLevel level)
    {
        return selectedDance is null
            ? []
            : connection.GetFigures(selectedDance, level).ToArray();
    }
}