using System.Diagnostics;
using System.Windows.Media;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.OptionalFigureSelection;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class OptionalFigureSelectionViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public bool IsSelected { get; set; }

    [Reactive]
    public DanceLevel Level { get; set; }

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public Brush Color { get; set; } = Brushes.Transparent;

    public OptionalFigureSelectionViewModel()
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