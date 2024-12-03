using ChoreoHelper.Search;

namespace ChoreoHelper.Shell.Behaviors;

public sealed class NavigateToSearchBehavior(SearchViewModel targetViewModel) : IBehavior<ShellViewModel>
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

        viewModel.GoToSearch = command;
    }
}