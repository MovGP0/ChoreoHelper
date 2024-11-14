using System.Reactive;
using DynamicData.Binding;
using OneOf.Types;

namespace ChoreoHelper.ViewModels;

public sealed class TransitionViewModel : ReactiveObject
{
    public TransitionViewModel()
    {
        if (this.IsInDesignMode())
        {
            FromFigureName = "FromFigureName";
            ToFigureName = "ToFigureName";
            SelectedDistance = new()
            {
                Description = "Not reachable",
                Distance = new None()
            };

            SelectedRestriction = new()
            {
                Description = "Allowed in all classes",
                Restriction = CompetitionRestriction.AllowedInAllClasses
            };
        }

        Distances.Add(new()
        {
            Description = "Not reachable",
            Distance = new None()
        });

        Distances.Add(new()
        {
            Description = "Reachable without modification",
            Distance = 1
        });

        Distances.Add(new()
        {
            Description = "Reachable with modification",
            Distance = 2
        });

        Restrictions.Add(new()
        {
            Description = "Allowed in all classes",
            Restriction = CompetitionRestriction.AllowedInAllClasses
        });
            
        Restrictions.Add(new()
        {
            Description = "Not allowed in class D, C, B, A",
            Restriction = CompetitionRestriction.NotAllowedInClassA
        });

        Restrictions.Add(new()
        {
            Description = "Not allowed in class D, C, B",
            Restriction = CompetitionRestriction.NotAllowedInClassB
        });

        Restrictions.Add(new()
        {
            Description = "Not allowed in class D, C",
            Restriction = CompetitionRestriction.NotAllowedInClassC
        });

        Restrictions.Add(new()
        {
            Description = "Not allowed in class D",
            Restriction = CompetitionRestriction.NotAllowedInClassD
        });
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