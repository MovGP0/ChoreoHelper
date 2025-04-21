using ChoreoHelper.CreateFigure;
using ChoreoHelper.TransitionEditor.Events;

namespace ChoreoHelper.TransitionEditor.Behaviors;

public sealed class ShowCreateFigureBehavior(
    ISubscriber<ShowCreateFigureCommand> showCreateFigureSubscriber)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        showCreateFigureSubscriber
            .Subscribe(args =>
            {
                var createFigureViewModel = new CreateFigureViewModel
                {
                    DanceName = args.Dance.Name
                };

                viewModel.EditViewModel = createFigureViewModel;
                viewModel.IsEditViewOpen = true;
            })
            .DisposeWith(disposables);
    }
}