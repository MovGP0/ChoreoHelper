using ChoreoHelper.Distance;
using ChoreoHelper.Restriction;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Transition;

public sealed class TransitionViewModel : ReactiveObject, IActivatableViewModel
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
    public string DanceName { get; set; } = string.Empty;

    [Reactive]
    public string FromFigureName { get; set; } = string.Empty;
    
    [Reactive]
    public string ToFigureName { get; set; } = string.Empty;

    public ObservableCollectionExtended<DistanceViewModel> Distances { get; } = new();

    [Reactive]
    public DistanceViewModel? SelectedDistance { get; set; }

    public ObservableCollectionExtended<RestrictionViewModel> Restrictions { get; } = new();

    [Reactive]
    public RestrictionViewModel? SelectedRestriction { get; set; }

    /// <summary>
    /// Navigates back to the previous view.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit> NavigateBack { get; set; } = DisabledCommand.Instance;

    /// <summary>
    /// Save the changes and navigate back to previous view.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit> SaveAndNavigateBack { get; set; } = DisabledCommand.Instance;

    public ViewModelActivator Activator { get; } = new();
}