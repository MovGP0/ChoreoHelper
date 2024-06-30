using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadOptionalFiguresBehavior(IDanceFiguresRepository connection) : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var optionalFigures = new SourceCache<OptionalFigureSelectionViewModel, string>(vm => vm.Hash);

        optionalFigures
            .Connect()
            .Bind(viewModel.OptionalFigures)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
            .Select(_ => viewModel)
            .Select(vm =>
            {
                if (!IsValid(vm))
                {
                    return [];
                }

                var hashesToIgnore = vm.RequiredFigures
                    .Where(r => r.IsSelected)
                    .Select(e => e.Hash)
                    .ToHashSet();

                return connection
                    .GetFigures(vm.SelectedDance, vm.GetLevels())
                    .Where(e => !hashesToIgnore.Contains(e.Hash))
                    .ToArray();
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Subscribe(loadedFigures =>
            {
                foreach (var loadedFigure in loadedFigures)
                {
                    var vm = ToViewModel(loadedFigure);
                    optionalFigures.AddOrUpdate(vm);
                }

                var toDelete = optionalFigures.Keys
                    .Except(loadedFigures.Select(e => e.Hash));

                optionalFigures.RemoveKeys(toDelete);
            })
            .DisposeWith(disposables);
    }

    private static bool IsValid(MainWindowViewModel vm)
    {
        return vm.SelectedDance is not null
               && vm.RequiredFigures.Count > 0
               && vm.Levels.Count > 0
               && !vm.RequiredFigures.All(r => r.IsSelected);
    }

    private static IObservable<Unit> Observe(MainWindowViewModel viewModel)
    {
        var obs0 = viewModel
            .WhenAnyValue(vm => vm.SelectedDance)
            .Select(_ => Unit.Default);

        var obs1 = MessageBus.Current
            .Listen<RequiredFigureUpdated>()
            .Select(_ => Unit.Default);

        var obs2 = MessageBus.Current
            .Listen<LevelChanged>()
            .Select(_ => Unit.Default);

        return obs0.Merge(obs1).Merge(obs2);
    }

    [Pure]
    private static OptionalFigureSelectionViewModel ToViewModel(DanceStepNodeInfo loadedFigure)
    {
        var vm = Locator.Current.GetRequiredService<OptionalFigureSelectionViewModel>();
        vm.Hash = loadedFigure.Hash;
        vm.Name = loadedFigure.Name;
        vm.IsSelected = true;
        vm.Level = loadedFigure.Level;
        return vm;
    }
}