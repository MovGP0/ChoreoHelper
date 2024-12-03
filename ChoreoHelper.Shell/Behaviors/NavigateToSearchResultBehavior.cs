using ChoreoHelper.SearchResult;

namespace ChoreoHelper.Shell.Behaviors;

public sealed class NavigateToSearchResultBehavior(
    SearchResultViewModel targetViewModel,
    ISubscriber<Messages.FoundChoreographies> foundChoreographiesSubscriber)
    : IBehavior<ShellViewModel>
{
    public void Activate(ShellViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create(() => {}).DisposeWith(disposables);

        command
            .Subscribe(_ =>
            {
                viewModel.Router.Navigate.Execute(targetViewModel);
            })
            .DisposeWith(disposables);

        viewModel.GoToSearchResult = command;

        foundChoreographiesSubscriber
            .Subscribe(cs =>
            {
                viewModel.Router.Navigate.Execute(targetViewModel);
            })
            .DisposeWith(disposables);
    }
}