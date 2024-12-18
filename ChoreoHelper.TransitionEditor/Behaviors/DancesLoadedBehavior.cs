using ChoreoHelper.Entities;
using ChoreoHelper.Messages;

namespace ChoreoHelper.TransitionEditor.Behaviors;

public sealed class DancesLoadedBehavior(ISubscriber<DataLoadedEvent> dataLoadedSubscriber)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        dataLoadedSubscriber
            .Subscribe(e => UpdateDances(viewModel, e.Dances))
            .DisposeWith(disposables);
    }

    private static void UpdateDances(TransitionEditorViewModel viewModel, ICollection<Dance> dances)
    {
        viewModel.SelectedDance = null;
        viewModel.Dances.Clear();
        viewModel.Figures.Clear();
        viewModel.Transitions = new DanceFigureTransition[0, 0];

        foreach (var dance in dances)
        {
            viewModel.Dances.Add(dance);
        }

        viewModel.SelectedDance = dances
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .First();
    }
}