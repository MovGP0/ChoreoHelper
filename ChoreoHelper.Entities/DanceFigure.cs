namespace ChoreoHelper.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class DanceFigure(Dance dance, string name, DanceLevel level, CompetitionRestriction restriction)
{
    [Pure]
    public Dance Dance { get; } = dance;

    [Pure]
    public string Name { get; } = name;

    public string Hash => Dance.Name + '|' + Name;

    [Pure]
    public DanceLevel Level { get; } = level;

    public CompetitionRestriction Restriction { get; } = restriction;

    [Pure]
    private string DebuggerDisplay => $"{Dance} {Name} {Level}";
}