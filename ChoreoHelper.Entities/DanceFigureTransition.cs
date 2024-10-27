using System.Diagnostics;
using QuikGraph;

namespace ChoreoHelper.Entities;

/// <summary>
/// Specifies a transition between two dance figures.
/// </summary>
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