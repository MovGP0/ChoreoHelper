using ChoreoHelper.Messages;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.SearchResult;

public sealed class CloseDrawerOnChoreographiesFoundBehavior : IBehavior<SearchResultViewModel>
{
    public void Activate(SearchResultViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel.Choreographies
            .WhenAnyPropertyChanged()
            .Subscribe(_ => MessageBus.Current.SendMessage(new CloseDrawer()))
            .DisposeWith(disposables);
    }
}