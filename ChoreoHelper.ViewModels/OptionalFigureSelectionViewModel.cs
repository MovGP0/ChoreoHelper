using System.Reactive.Disposables;
using ChoreoHelper.Database;
using ReactiveUI;
using Splat;

namespace ChoreoHelper.ViewModels;

public sealed class OptionalFigureSelectionViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    /// <summary>
    /// The hash code of the dance step.
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public DanceLevel Level { get; set; }
    public int LevelSort => (int)Level;

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

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
}