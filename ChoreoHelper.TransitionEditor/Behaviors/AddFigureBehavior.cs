using ChoreoHelper.TransitionEditor.Events;

namespace ChoreoHelper.TransitionEditor.Behaviors;

/// <summary>
/// Defines the behavior of the <see cref="TransitionEditorViewModel.AddFigure"/> command.
/// </summary>
public sealed class AddFigureBehavior(
    IPublisher<ShowCreateFigureCommand> addFigurePublisher)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(
        TransitionEditorViewModel viewModel,
        CompositeDisposable disposables)
    {
        var canAddFigure = viewModel
            .WhenAnyValue(e => e.SelectedDance)
            .Select(dance => dance is not null);

        var addFigure = ReactiveCommand
            .Create<Unit, Unit>(_ => Unit.Default, canAddFigure)
            .DisposeWith(disposables);

        addFigure
            .Subscribe(_ =>
            {
                if (viewModel.SelectedDance is { } dance)
                {
                    addFigurePublisher.Publish(new(dance));
                }
            })
            .DisposeWith(disposables);

        viewModel.AddFigure = addFigure;
        Disposable.Create(() => viewModel.AddFigure = DisabledCommand.Instance).DisposeWith(disposables);
    }
}