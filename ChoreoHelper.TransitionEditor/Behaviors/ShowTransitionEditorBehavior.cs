using ChoreoHelper.Entities;
using ChoreoHelper.Transition;
using ChoreoHelper.TransitionEditor.Events;
using DynamicData.Kernel;

namespace ChoreoHelper.TransitionEditor.Behaviors;

public sealed class ShowTransitionEditorBehavior(
    ISubscriber<ShowTransitionEditorCommand> showTransitionEditorSubscriber)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        showTransitionEditorSubscriber.
            Subscribe(args =>
            {
                ShowTransitionEditor(viewModel, args.Transition);
            })
            .DisposeWith(disposables);
    }

    private void ShowTransitionEditor(
        TransitionEditorViewModel viewModel,
        DanceFigureTransition transition)
    {
        if (viewModel.SelectedDance is not { } selectedDance)
        {
            return;
        }

        var transitionViewModel = new TransitionViewModel
        {
            DanceName = selectedDance.Name,
            FromFigureName = transition.Source.Name,
            ToFigureName = transition.Target.Name
        };

        var result = transitionViewModel.Distances
            .FirstOrOptional(d => DistanceComparer.Default.Equals(d.Distance, transition.Distance));

        transitionViewModel.SelectedDistance = result.HasValue
            ? result.Value
            : transitionViewModel.Distances.First();

        var rest = transitionViewModel.Restrictions
            .FirstOrOptional(r => r.Restriction == transition.Restriction);

        transitionViewModel.SelectedRestriction = rest.HasValue
            ? rest.Value
            :  transitionViewModel.Restrictions.First();

        viewModel.EditViewModel = transitionViewModel;
        viewModel.IsEditViewOpen = true;
    }
}