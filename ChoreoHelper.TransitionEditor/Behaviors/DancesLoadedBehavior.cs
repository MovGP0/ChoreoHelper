using ChoreoHelper.Entities;

namespace ChoreoHelper.TransitionEditor.Behaviors;

public sealed class DancesLoadedBehavior(DancesCache dancesCache)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        dancesCache
            .Connect()
            .Subscribe(_ => UpdateDances(viewModel, dancesCache.Items.ToImmutableArray()))
            .DisposeWith(disposables);
    }

    private static void UpdateDances(TransitionEditorViewModel viewModel, IReadOnlyCollection<Dance> dances)
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
            .FirstOrDefault();
    }
}