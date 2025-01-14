using System.Diagnostics;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Dance;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed partial class DanceViewModel : ReactiveObject
{
    /// <summary>
    /// The hash code of the dance.
    /// </summary>
    [Reactive]
    private string _hash = string.Empty;

    [Reactive]
    private string _name = string.Empty;

    [Reactive]
    private string _category = string.Empty;

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