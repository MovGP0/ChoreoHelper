using System.Diagnostics;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Dance;

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
            InitializeDesignModeData();
        }
    }

    private void InitializeDesignModeData()
    {
        Name = "Dance Name";
        Category = "Latin";
    }

    private string DebuggerDisplay => $"[{Category}] {Name}";
}