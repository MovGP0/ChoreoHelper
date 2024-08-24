using System.Diagnostics;

namespace ChoreoHelper.ViewModels;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class DanceViewModel : ReactiveObject
{
    /// <summary>
    /// The hash code of the dance.
    /// </summary>
    [Reactive]
    public string Hash { get; set; } = string.Empty;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public string Category { get; set; } = string.Empty;

    public DanceViewModel()
    {
        if (this.IsInDesignMode())
        {
            Name = "Dance Name";
            Category = "Latin";
        }
    }

    private string DebuggerDisplay => $"[{Category}] {Name}";
}