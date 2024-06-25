namespace ChoreoHelper.Graph.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class DanceFigureTransition(DanceFigure source, DanceFigure target, float distance)
    : IEdge<DanceFigure>
{
    [Pure]
    public DanceFigure Source { get; } = source;

    [Pure]
    public DanceFigure Target { get; } = target;

    [Pure]
    public float Distance { get; } = distance;

    [Pure]
    private string DebuggerDisplay => $"{Source} -> {Target} ({Distance})";
}