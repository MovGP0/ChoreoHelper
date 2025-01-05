using System.Diagnostics;
using System.Windows.Media;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.RequiredFigureSelection;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class RequiredFigureSelectionViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public bool IsSelected { get; set; }

    [Reactive]
    public DanceLevel Level { get; set; } = DanceLevel.Undefined;

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public Brush Color { get; set; } = Brushes.Transparent;

    public RequiredFigureSelectionViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.ActivateBehaviors(Disposables);
    }

    private void InitializeDesignModeData()
    {
        Name = "Dance Figure Name";
        IsSelected = true;
    }

    public void Dispose() => Disposables.Dispose();

    private string DebuggerDisplay => $"[{Level}] {Name} {IsSelected}";
}