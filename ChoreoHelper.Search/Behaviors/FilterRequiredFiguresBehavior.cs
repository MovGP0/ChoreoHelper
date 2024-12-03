using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using ChoreoHelper.RequiredFigureSelection;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class FilterRequiredFiguresBehavior(
    ISubscriber<LevelChanged> levelChangedSubscriber)
    : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var requiredFiguresFiltered = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Name)
            .DisposeWith(disposables);

        requiredFiguresFiltered
            .Connect()
            .Bind(viewModel.RequiredFiguresFiltered)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
             .Select(_ => viewModel)
             .Subscribe(vm =>
             {
                 var searchText = vm.SearchText;
                 var figures = viewModel
                     .RequiredFigures
                     .Where(rf => string.IsNullOrEmpty(searchText) || rf.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                     .Where(of => FilterByLevel(of, vm.GetLevels()))
                     .ToList();

                 requiredFiguresFiltered.Update(figures);
             })
             .DisposeWith(disposables);
    }

    private static bool FilterByLevel(RequiredFigureSelectionViewModel figure, DanceLevel levelFilter)
    {
        return figure.Level.IsFlagSet(DanceLevel.Advanced) && levelFilter.IsFlagSet(DanceLevel.Advanced)
               || figure.Level.IsFlagSet(DanceLevel.Gold) && levelFilter.IsFlagSet(DanceLevel.Gold)
               || figure.Level.IsFlagSet(DanceLevel.Silver) && levelFilter.IsFlagSet(DanceLevel.Silver)
               || figure.Level.IsFlagSet(DanceLevel.Bronze) && levelFilter.IsFlagSet(DanceLevel.Bronze);
    }

    private IObservable<Unit> Observe(SearchViewModel viewModel)
    {
        var listChanged = viewModel.RequiredFigures
            .OnCollectionChanged()
            .Select(_ => Unit.Default);

        var searchTextChanged = viewModel
            .WhenAnyValue(vm => vm.SearchText)
            .Select(_ => Unit.Default);

        var levelChanged = levelChangedSubscriber.AsObservable()
            .Select(_ => Unit.Default);

        return searchTextChanged
            .Merge(listChanged)
            .Merge(levelChanged);
    }
}