using ChoreoHelper.Messages;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.SearchResult;

public sealed class CloseDrawerOnChoreographiesFoundBehavior(
    IPublisher<CloseDrawer> closeDrawerPublisher)
    : IBehavior<SearchResultViewModel>
{
    public void Activate(SearchResultViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel.Choreographies
            .WhenAnyPropertyChanged()
            .Subscribe(_ => closeDrawerPublisher.Publish(new CloseDrawer()))
            .DisposeWith(disposables);
    }
}