using ChoreoHelper.Distance;
using ChoreoHelper.Restriction;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Transition;

public sealed class TransitionViewModel : ReactiveObject
{
    public TransitionViewModel()
    {
        Distances.AddRange(DistancesViewModelFactory.Create());
        Restrictions.AddRange(RestrictionViewModelFactory.Create());

        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }
    }

    private void InitializeDesignModeData()
    {
        FromFigureName = "FromFigureName";
        ToFigureName = "ToFigureName";
        SelectedDistance = Distances.First();
        SelectedRestriction = Restrictions.First();
    }

    [Reactive]
    public string FromFigureName { get; set; } = string.Empty;
    
    [Reactive]
    public string ToFigureName { get; set; } = string.Empty;

    public IObservableCollection<DistanceViewModel> Distances { get; }
        = new ObservableCollectionExtended<DistanceViewModel>();

    [Reactive]
    public DistanceViewModel? SelectedDistance { get; set; }

    public IObservableCollection<RestrictionViewModel> Restrictions { get; }
        = new ObservableCollectionExtended<RestrictionViewModel>();

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
}