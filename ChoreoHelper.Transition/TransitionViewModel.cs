using ChoreoHelper.Distance;
using ChoreoHelper.Restriction;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Transition;

public sealed partial class TransitionViewModel : ReactiveObject, IActivatableViewModel
{
    public TransitionViewModel()
    {
        Distances.AddRange(DistancesViewModelFactory.Create());
        Restrictions.AddRange(RestrictionViewModelFactory.Create());

        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.WhenActivated(disposables =>
        {
            foreach (var behavior in Locator.Current.GetServices<IBehavior<TransitionViewModel>>())
            {
                behavior.Activate(this, disposables);
            }
        });
    }

    private void InitializeDesignModeData()
    {
        DanceName = "Viennese Waltz";
        FromFigureName = "FromFigureName";
        ToFigureName = "ToFigureName";
        SelectedDistance = Distances.First();
        SelectedRestriction = Restrictions.First();
    }

    [Reactive]
    private string _danceName = string.Empty;

    [Reactive]
    private string _fromFigureName = string.Empty;

    [Reactive]
    private string _toFigureName = string.Empty;

    public ObservableCollectionExtended<DistanceViewModel> Distances { get; } = new();

    [Reactive]
    private DistanceViewModel? _selectedDistance;

    public ObservableCollectionExtended<RestrictionViewModel> Restrictions { get; } = new();

    [Reactive]
    private RestrictionViewModel? _selectedRestriction;

    /// <summary>
    /// Navigates back to the previous view.
    /// </summary>
    [Reactive]
    private ReactiveCommand<Unit, Unit> _navigateBack = DisabledCommand.Instance;

    /// <summary>
    /// Save the changes and navigate back to previous view.
    /// </summary>
    [Reactive]
    private ReactiveCommand<Unit, Unit> _saveAndNavigateBack = DisabledCommand.Instance;

    public ViewModelActivator Activator { get; } = new();
}