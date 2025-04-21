using ChoreoHelper.Messages;

namespace ChoreoHelper.CreateFigure.Behaviors;

public sealed class NavigateBackBehavior(
    IPublisher<CloseCreateFigure> closeCreateFigurePublisher)
    : IBehavior<CreateFigureViewModel>
{
    public void Activate(CreateFigureViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default).DisposeWith(disposables);

        viewModel.NavigateBack = command;
        Disposable.Create(() => viewModel.NavigateBack = DisabledCommand.Instance).DisposeWith(disposables);

        command
            .Subscribe(_ => closeCreateFigurePublisher.Publish(new()))
            .DisposeWith(disposables);
    }
}