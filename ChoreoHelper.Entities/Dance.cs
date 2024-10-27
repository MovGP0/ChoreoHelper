using System.Diagnostics;

namespace ChoreoHelper.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class Dance(string category, string name)
{
    [Pure]
    public string Category { get; } = category;

    [Pure]
    public string Name { get; } = name;

    [Pure]
    public IList<DanceFigureTransition> Transitions { get; } = new List<DanceFigureTransition>();
    
    [Pure]
    public IList<DanceFigure> Figures { get; } = new List<DanceFigure>();

    [Pure]
    private string DebuggerDisplay => $"{Category} {Name}";
}