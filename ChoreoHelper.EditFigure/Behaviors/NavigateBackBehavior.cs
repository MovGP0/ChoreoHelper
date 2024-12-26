using ChoreoHelper.Messages;

namespace ChoreoHelper.EditFigure.Behaviors;

public sealed class NavigateBackBehavior(IPublisher<CloseEditFigure> closeEditFigurePublisher) : IBehavior<EditFigureViewModel>
{
    public void Activate(EditFigureViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default).DisposeWith(disposables);

        viewModel.NavigateBack = command;
        Disposable.Create(() => viewModel.NavigateBack = DisabledCommand.Instance).DisposeWith(disposables);

        command
            .Subscribe(_ => closeEditFigurePublisher.Publish(new()))
            .DisposeWith(disposables);
    }
}