using ChoreoHelper.Messages;

namespace ChoreoHelper.Transition.Behaviors;

public sealed class NavigateBackBehavior(IPublisher<CloseEditTransition> closeEditTransitionPublisher) : IBehavior<TransitionViewModel>
{
    public void Activate(TransitionViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default).DisposeWith(disposables);

        viewModel.NavigateBack = command;
        Disposable.Create(() => viewModel.NavigateBack = DisabledCommand.Instance).DisposeWith(disposables);

        command
            .Subscribe(_ => closeEditTransitionPublisher.Publish(new()))
            .DisposeWith(disposables);
    }
}