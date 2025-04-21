using ChoreoHelper.EditFigure;
using ChoreoHelper.Entities;
using ChoreoHelper.TransitionEditor.Events;
using DynamicData.Kernel;

namespace ChoreoHelper.TransitionEditor.Behaviors;

public sealed class ShowFigureEditorBehavior(
    ISubscriber<ShowFigureEditorCommand> showFigureEditorSubscriber)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(
        TransitionEditorViewModel viewModel,
        CompositeDisposable disposables)
    {
        showFigureEditorSubscriber
            .Subscribe(args =>
            {
                ShowFigureEditor(viewModel, args.Figure);
            })
            .DisposeWith(disposables);
    }

    private void ShowFigureEditor(TransitionEditorViewModel viewModel, DanceFigure figure)
    {
        if (viewModel.SelectedDance is not {} selectedDance)
        {
            return;
        }

        var figureViewModel = new EditFigureViewModel
        {
            DanceName = selectedDance.Name,
            Name = figure.Name,
            Hash = figure.Hash
        };

        var level = figureViewModel.Levels
            .FirstOrOptional(l => l.Level == figure.Level);

        figureViewModel.Level = level.HasValue
            ? level.Value
            : figureViewModel.Level = figureViewModel.Levels.First();

        var restriction = figureViewModel.Restrictions
            .FirstOrOptional(r => r.Restriction == figure.Restriction);

        figureViewModel.Restriction = restriction.HasValue
            ? restriction.Value
            : figureViewModel.Restrictions.First();

        viewModel.EditViewModel = figureViewModel;
        viewModel.IsEditViewOpen = true;
    }
}