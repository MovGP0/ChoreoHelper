namespace ChoreHelper.Editor.Model;

public sealed class Dance
{
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<DanceFigure> Figures { get; set; } = new();
    public List<Transition> Transitions { get; set; } = new();
}