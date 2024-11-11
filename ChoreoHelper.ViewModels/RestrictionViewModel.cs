using System.Diagnostics;

namespace ChoreoHelper.ViewModels;

[DebuggerDisplay("{Description}")]
public sealed class RestrictionViewModel : ReactiveObject
{
    [Reactive]
    public CompetitionRestriction Restriction { get; set; }

    [Reactive]
    public string Description { get; set; } = string.Empty;
}