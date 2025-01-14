using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Restriction;

[DebuggerDisplay("{Description}")]
public sealed partial class RestrictionViewModel : ReactiveObject
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
    private CompetitionRestriction _restriction = CompetitionRestriction.AllowedInAllClasses;

    [Reactive]
    private string _description = string.Empty;
}