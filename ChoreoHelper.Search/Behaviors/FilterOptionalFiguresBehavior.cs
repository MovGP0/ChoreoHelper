using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using ChoreoHelper.OptionalFigureSelection;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class FilterOptionalFiguresBehavior(
    ISubscriber<RequiredFigureUpdated> requiredFigureUpdatedSubscriber,
    ISubscriber<LevelChanged> levelChangedSubscriber)
    : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var optionalFiguresFiltered = new SourceCache<OptionalFigureSelectionViewModel, string>(vm => vm.Name)
            .DisposeWith(disposables);

        optionalFiguresFiltered
            .Connect()
            .Bind(viewModel.OptionalFiguresFiltered)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
            .Select(_ => viewModel)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(vm =>
            {
                var selectedRequiredFigureHashes = vm.RequiredFiguresFiltered
                    .Where(rf => rf.IsSelected)
                    .Select(rf => rf.Name)
                    .ToImmutableHashSet();

                var searchText = vm.SearchText;
                var figures = viewModel
                    .OptionalFigures
                    .Where(rf => string.IsNullOrEmpty(searchText) || rf.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                    .Where(of => !selectedRequiredFigureHashes.Contains(of.Name))
                    .Where(of => FilterByLevel(of, vm.GetLevels()))
                    .ToImmutableArray();

                optionalFiguresFiltered.Update(figures);
            })
            .DisposeWith(disposables);
    }

    private static bool FilterByLevel(OptionalFigureSelectionViewModel figure, DanceLevel levelFilter)
    {
        return figure.Level.IsFlagSet(DanceLevel.Advanced) && levelFilter.IsFlagSet(DanceLevel.Advanced)
               || figure.Level.IsFlagSet(DanceLevel.Gold) && levelFilter.IsFlagSet(DanceLevel.Gold)
               || figure.Level.IsFlagSet(DanceLevel.Silver) && levelFilter.IsFlagSet(DanceLevel.Silver)
               || figure.Level.IsFlagSet(DanceLevel.Bronze) && levelFilter.IsFlagSet(DanceLevel.Bronze);
    }

    private IObservable<Unit> Observe(SearchViewModel viewModel)
    {
        var lastCount = 0;
        var listChanged = viewModel.OptionalFigures
            .WhenPropertyChanged(x => x.Count)
            .Where(_ => viewModel.OptionalFigures.Count != lastCount)
            .Do(_ => lastCount = viewModel.OptionalFigures.Count)
            .Select(_ => Unit.Default);

        var searchTextChanged = viewModel
            .WhenAnyValue(
                vm => vm.SearchText,
                vm => vm.SelectedDance)
            .Select(_ => Unit.Default);

        var requiredFigureChanged = requiredFigureUpdatedSubscriber.AsObservable()
            .Select(_ => Unit.Default);

        var levelChanged = levelChangedSubscriber.AsObservable()
            .Select(_ => Unit.Default);

        return searchTextChanged
            .Merge(listChanged)
            .Merge(requiredFigureChanged)
            .Merge(levelChanged);
    }
}