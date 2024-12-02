using System.Diagnostics;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.OptionalFigureSelection;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class OptionalFigureSelectionViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    /// <summary>
    /// The hash code of the dance step.
    /// </summary>
    [Reactive]
    public string Hash { get; set; } = string.Empty;

    [Reactive]
    public bool IsSelected { get; set; }

    [Reactive]
    public DanceLevel Level { get; set; }

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    public OptionalFigureSelectionViewModel()
    {
        if (this.IsInDesignMode())
        {
            Name = "Dance Figure Name";
            IsSelected = true;
        }

        foreach (var behavior in Locator.Current.GetServices<IBehavior<OptionalFigureSelectionViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    public void Dispose() => Disposables.Dispose();

    private string DebuggerDisplay => $"[{Level}] {Name} {IsSelected}";
}