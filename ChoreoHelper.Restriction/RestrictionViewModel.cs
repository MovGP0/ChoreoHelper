using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Restriction;

[DebuggerDisplay("{Description}")]
public sealed class RestrictionViewModel : ReactiveObject
{
    public RestrictionViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }
    }

    private void InitializeDesignModeData()
    {
        Restriction = CompetitionRestriction.AllowedInAllClasses;
        Description = "Allowed in all classes";
    }

    [Reactive]
    public CompetitionRestriction Restriction { get; set; } = CompetitionRestriction.AllowedInAllClasses;

    [Reactive]
    public string Description { get; set; } = string.Empty;
}