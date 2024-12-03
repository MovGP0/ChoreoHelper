using ChoreoHelper.Choreography;
using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using ReactiveUI.Extensions;

namespace ChoreoHelper.SearchResult.Behaviors;

public sealed class LoadChoreographyBehavior(
    ISubscriber<FoundChoreographies> foundChoreographiesSubscriber)
    : IBehavior<SearchResultViewModel>
{
    public void Activate(SearchResultViewModel viewModel, CompositeDisposable disposables)
    {
        var choreographies = new SourceList<ChoreographyViewModel>()
            .DisposeWith(disposables);

        choreographies
            .Connect()
            .Bind(viewModel.Choreographies)
            .Subscribe()
            .DisposeWith(disposables);

        foundChoreographiesSubscriber
            .Subscribe(cs =>
            {
                var choreographyItems = cs.Items;

                var viewModels = choreographyItems
                    .Select(ToChoreographyViewModel)
                    .ToImmutableArray();

                choreographies.Update(viewModels);
            })
            .DisposeWith(disposables);
    }

    [Pure]
    private static ChoreographyViewModel ToChoreographyViewModel(DanceStepNodeInfo[] item)
    {
        ChoreographyViewModel choreographyViewModel = new()
        {
            Rating = 1f / item.Length * 10f
        };
        choreographyViewModel.Activator.Activate();
        ListEx.AddRange(choreographyViewModel.Figures, item);
        return choreographyViewModel;
    }
}