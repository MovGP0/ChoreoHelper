using System.Collections.Immutable;
using ChoreoHelper.Entities;
using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.SearchResult;

public sealed class LoadChoreographyBehavior: IBehavior<SearchResultViewModel>
{
    public void Activate(SearchResultViewModel viewModel, CompositeDisposable disposables)
    {
        var choreographies = new SourceList<ChoreographyViewModel>()
            .DisposeWith(disposables);

        choreographies
            .Connect()
            .Bind(viewModel.Choreographies)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        MessageBus.Current
            .Listen<FoundChoreographies>()
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe(cs =>
            {
                var choreographyItems = cs.Items;

                choreographies.Clear();

                var viewModels = choreographyItems
                    .Select(ToChoreographyViewModel)
                    .ToImmutableArray();

                choreographies.AddRange(viewModels);
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
        choreographyViewModel.Figures.AddRange(item);
        return choreographyViewModel;
    }
}