using DynamicData.Binding;

namespace ChoreoHelper.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class Dance(string category, string name)
{
    [Pure]
    public string Category { get; } = category;

    [Pure]
    public string Name { get; } = name;

    [Pure]
    public ObservableCollectionExtended<DanceFigureTransition> Transitions { get; } = new();

    [Pure]
    public ObservableCollectionExtended<DanceFigure> Figures { get; } = new();

    [Pure]
    private string DebuggerDisplay => $"{Category} {Name}";
}