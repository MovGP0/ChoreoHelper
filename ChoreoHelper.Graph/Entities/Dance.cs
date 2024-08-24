namespace ChoreoHelper.Graph.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class Dance(string category, string name)
{
    [Pure]
    public string Category { get; } = category;

    [Pure]
    public string Name { get; } = name;

    [Pure]
    private string DebuggerDisplay => $"{Category} {Name}";
}