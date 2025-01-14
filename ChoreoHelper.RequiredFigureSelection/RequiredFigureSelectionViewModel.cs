using System.Diagnostics;
using System.Windows.Media;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.RequiredFigureSelection;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed partial class RequiredFigureSelectionViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    private bool _isSelected;

    [Reactive]
    private DanceLevel _level = DanceLevel.Undefined;

    public int LevelSort => (int)Level;

    [Reactive]
    private string _name = string.Empty;

    [Reactive]
    private Brush _color = Brushes.Transparent;

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