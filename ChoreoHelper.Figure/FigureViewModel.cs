using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Figure;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed partial class FigureViewModel : ReactiveObject
{
    [Reactive]
    private DanceLevel _level = DanceLevel.Undefined;

    public int LevelSort => (int)Level;

    [Reactive]
    private string _name = string.Empty;

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