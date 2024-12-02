using System.Diagnostics;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Restriction;

[DebuggerDisplay("{Description}")]
public sealed class RestrictionViewModel : ReactiveObject
{
    [Reactive]
    public CompetitionRestriction Restriction { get; set; }

    [Reactive]
    public string Description { get; set; } = string.Empty;
}