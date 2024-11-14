using QuikGraph;

namespace ChoreoHelper.Entities;

/// <summary>
/// Specifies a transition between two dance figures.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class DanceFigureTransition(DanceFigure source, DanceFigure target, OneOf<float, None> distance, CompetitionRestriction restriction)
    : IEdge<DanceFigure>
{
    [Pure]
    public DanceFigure Source { get; } = source;

    [Pure]
    public DanceFigure Target { get; } = target;

    [Pure]
    public OneOf<float, None> Distance { get; } = distance;

    [Pure]
    public CompetitionRestriction Restriction { get; } = restriction;

    [Pure]
    private string DebuggerDisplay => $"{Source} -> {Target} ({Distance})";
}