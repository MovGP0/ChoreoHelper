using System.Diagnostics;

namespace ChoreoHelper.ViewModels;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class FigureViewModel : ReactiveObject
{
    /// <summary>
    /// The hash code of the dance step.
    /// </summary>
    [Reactive]
    public string Hash { get; set; } = string.Empty;

    [Reactive]
    public DanceLevel Level { get; set; } = DanceLevel.Undefined;

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    public FigureViewModel()
    {
        if (this.IsInDesignMode())
        {
            Name = "Dance Figure Name";
        }
    }

    private string DebuggerDisplay => $"[{Level}] {Name}";
}