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
    private static ChoreographyViewModel ToChoreographyViewModel(DanceStepNodeInfo[] items)
    {
        ChoreographyViewModel choreographyViewModel = new()
        {
            Rating = 1f / items.Length * 10f
        };

        var itemModels = items.Select(ToViewModel);
        choreographyViewModel.Figures.AddRange(itemModels);
        
        choreographyViewModel.Activator.Activate();
        return choreographyViewModel;
    }

    private static ChoreographyItemViewModel ToViewModel(DanceStepNodeInfo item)
    {
        var vm = new ChoreographyItemViewModel
        {
            Name = item.Name,
            Level = item.Level
        };
        _ = vm.Activator.Activate();
        return vm;
    }
}