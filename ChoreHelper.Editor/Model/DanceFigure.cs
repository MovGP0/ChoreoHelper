using ChoreoHelper.Entities;

namespace ChoreHelper.Editor.Model;

public sealed class DanceFigure
{
    public string Name { get; set; } = string.Empty;
    public DanceLevel Level { get; set; } = DanceLevel.Undefined;
}