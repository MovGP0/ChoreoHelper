using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Figure;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class FigureViewModel : ReactiveObject
{
    [Reactive]
    public DanceLevel Level { get; set; } = DanceLevel.Undefined;

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    public FigureViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }
    }

    private void InitializeDesignModeData()
    {
        Name = "Dance Figure Name";
        Level = DanceLevel.Bronze;
    }

    private string DebuggerDisplay => $"[{Level}] {Name}";
}