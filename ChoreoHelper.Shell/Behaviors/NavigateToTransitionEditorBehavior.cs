using ChoreoHelper.TransitionEditor;

namespace ChoreoHelper.Shell.Behaviors;

public sealed class NavigateToTransitionEditorBehavior(TransitionEditorViewModel targetViewModel) : IBehavior<ShellViewModel>
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

        viewModel.GoToTransitionEditor = command;
    }
}