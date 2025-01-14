using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.LevelSelection;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed partial class LevelSelectionViewModel: ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public DanceLevel _level;

    public int LevelSort => (int)Level;

    [Reactive]
    public string _name = string.Empty;

    [Reactive]
    public bool _isSelected = true;

    public LevelSelectionViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.ActivateBehaviors(Disposables);
    }

    private void InitializeDesignModeData()
    {
        Level = DanceLevel.Bronze;
        Name = DanceLevel.Bronze.ToString();
    }

    public void Dispose() => Disposables.Dispose();

    private string DebuggerDisplay => $"{Name} ({Level})";
}