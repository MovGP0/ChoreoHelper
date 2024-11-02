using System.Reactive.Disposables;
using ChoreoHelper.Editor.ViewModels;
using ReactiveUI;

namespace ChoreoHelper.Editor.Behaviors;

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