using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.LevelSelection;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class LevelSelectionViewModel: ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public DanceLevel Level { get; set; }

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public bool IsSelected { get; set; } = true;

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