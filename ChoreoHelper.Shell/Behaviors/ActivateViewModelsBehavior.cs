using ChoreoHelper.Choreography;
using ChoreoHelper.Search;
using ChoreoHelper.SearchResult;
using ChoreoHelper.TransitionEditor;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Shell.Behaviors;

/// <summary>
/// Ensures that the view models are activated before navigating to them,
/// so they can receive updates.
/// </summary>
/// <param name="searchViewModel"></param>
public sealed class ActivateViewModelsBehavior(
    SearchViewModel searchViewModel,
    ChoreographyViewModel choreographyViewModel,
    TransitionEditorViewModel transitionEditorViewModel,
    SearchResultViewModel searchResultViewModel)
    : IBehavior<ShellViewModel>
{
    public void Activate(ShellViewModel viewModel, CompositeDisposable disposables)
    {
        searchViewModel.Activator.Activate();
        disposables.Add(() => searchViewModel.Activator.Deactivate());

        choreographyViewModel.Activator.Activate();
        disposables.Add(() => choreographyViewModel.Activator.Deactivate());

        transitionEditorViewModel.Activator.Activate();
        disposables.Add(() => transitionEditorViewModel.Activator.Deactivate());

        searchResultViewModel.Activator.Activate();
        disposables.Add(() => searchResultViewModel.Activator.Deactivate());
    }
}