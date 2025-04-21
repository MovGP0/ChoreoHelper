using ChoreoHelper.Messages;

namespace ChoreoHelper.TransitionEditor.Behaviors;

public sealed class CloseEditViewBehavior(
    ISubscriber<CloseEditTransition> closeEditTransitionSubscriber,
    ISubscriber<CloseEditFigure> closeEditFigureSubscriber,
    ISubscriber<CloseCreateFigure> closeCreateFigureSubscriber)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        closeEditTransitionSubscriber
            .AsObservable()
            .Subscribe(_ => CloseEditView())
            .DisposeWith(disposables);

        closeEditFigureSubscriber
            .AsObservable()
            .Subscribe(_ => CloseEditView())
            .DisposeWith(disposables);

        closeCreateFigureSubscriber
            .AsObservable()
            .Subscribe(_ => CloseEditView())
            .DisposeWith(disposables);

        void CloseEditView()
        {
            viewModel.IsEditViewOpen = false;
            viewModel.EditViewModel = null;
        }
    }
}