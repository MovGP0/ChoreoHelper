using System.Collections.Immutable;
using System.Collections.Specialized;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.Search;

public sealed class LoadOptionalFiguresBehavior(IDanceFiguresRepository connection) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var optionalFigures = new SourceCache<OptionalFigureSelectionViewModel, string>(vm => vm.Hash)
            .DisposeWith(disposables);

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
                    .GetFigures(vm.SelectedDance?.Name, vm.GetLevels())
                    .Where(e => !hashesToIgnore.Contains(e.Hash))
                    .ToArray();
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Subscribe(loadedFigures =>
            {
                var selectedRequiredFiguresHashes = viewModel.RequiredFigures
                    .Where(rf => rf.IsSelected)
                    .Select(e => e.Hash)
                    .ToImmutableHashSet();

                foreach (var loadedFigure in loadedFigures)
                {
                    var vm = ToViewModel(loadedFigure);
                    if (selectedRequiredFiguresHashes.Contains(vm.Hash))
                    {
                        continue;
                    }
                    optionalFigures.AddOrUpdate(vm);
                }

                var toDelete = optionalFigures.Keys
                    .Except(loadedFigures.Select(e => e.Hash))
                    .Concat(selectedRequiredFiguresHashes);

                optionalFigures.RemoveKeys(toDelete);
            })
            .DisposeWith(disposables);
    }

    private static bool IsValid(SearchViewModel vm)
    {
        return vm.SelectedDance is not null
               && vm.RequiredFigures.Count > 0
               && vm.Levels.Count > 0
               && !vm.RequiredFigures.All(r => r.IsSelected);
    }

    private static IObservable<Unit> Observe(SearchViewModel viewModel)
    {
        var figuresChanged = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => viewModel.Figures.CollectionChanged += handler,
                handler => viewModel.Figures.CollectionChanged -= handler)
            .Select(_ => Unit.Default);

        var requiredFigureChanged = MessageBus.Current
            .Listen<RequiredFigureUpdated>()
            .Select(_ => Unit.Default);

        return figuresChanged.Merge(requiredFigureChanged);
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