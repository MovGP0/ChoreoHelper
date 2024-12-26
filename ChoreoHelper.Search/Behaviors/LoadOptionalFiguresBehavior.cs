using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using ChoreoHelper.OptionalFigureSelection;
using ReactiveMarbles.ObservableEvents;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadOptionalFiguresBehavior(
    ISubscriber<RequiredFigureUpdated> requiredFigureUpdatedSubscriber,
    DancesCache dancesCache)
    : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var optionalFigures = new SourceCache<OptionalFigureSelectionViewModel, string>(vm => vm.Name)
            .DisposeWith(disposables);

        optionalFigures
            .Connect()
            .Bind(viewModel.OptionalFigures)
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
                    .Select(e => e.Name)
                    .ToHashSet();

                return dancesCache.Items
                    .Where(d => vm.SelectedDance is not null && d.Name == vm.SelectedDance?.Name)
                    .SelectMany(dance => dance.Figures)
                    .Where(e => !hashesToIgnore.Contains(e.Name))
                    .Select(danceFigure => new DanceStepNodeInfo(danceFigure.Name, danceFigure.Level))
                    .ToArray();
            })
            .Subscribe(loadedFigures =>
            {
                var selectedRequiredFiguresHashes = viewModel.RequiredFigures
                    .Where(rf => rf.IsSelected)
                    .Select(e => e.Name)
                    .ToImmutableHashSet();

                foreach (var loadedFigure in loadedFigures)
                {
                    var vm = ToViewModel(loadedFigure);
                    if (selectedRequiredFiguresHashes.Contains(vm.Name))
                    {
                        continue;
                    }
                    optionalFigures.AddOrUpdate(vm);
                }

                var toDelete = optionalFigures.Keys
                    .Except(loadedFigures.Select(e => e.Name))
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

    private IObservable<Unit> Observe(SearchViewModel viewModel)
    {
        var figuresChanged = viewModel.Figures
            .Events()
            .CollectionChanged
            .Throttle(TimeSpan.FromMilliseconds(50))
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Select(_ => Unit.Default);

        var requiredFigureChanged = requiredFigureUpdatedSubscriber.AsObservable()
            .Select(_ => Unit.Default);

        var dancesChanged = dancesCache
            .Connect()
            .Select(change => Unit.Default);

        return figuresChanged
            .Merge(requiredFigureChanged)
            .Merge(dancesChanged);
    }

    [Pure]
    private static OptionalFigureSelectionViewModel ToViewModel(DanceStepNodeInfo loadedFigure)
    {
        var vm = new OptionalFigureSelectionViewModel();
        vm.Name = loadedFigure.Name;
        vm.IsSelected = true;
        vm.Level = loadedFigure.Level;
        return vm;
    }
}