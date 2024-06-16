using System.Reactive.Disposables;
using ChoreoHelper.Database;
using ReactiveUI;
using Splat;

namespace ChoreoHelper.ViewModels;

public sealed class LevelSelectionViewModel: ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    private DanceLevel _level;
    public DanceLevel Level
    {
        set => this.RaiseAndSetIfChanged(ref _level, value);
        get => _level;
    }

    public int LevelSort => (int)Level;

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public LevelSelectionViewModel()
    {
        if (this.IsInDesignMode())
        {
            Level = DanceLevel.Bronze;
            Name = DanceLevel.Bronze.ToString();
        }

        foreach (var behavior in Locator.Current.GetServices<IBehavior<LevelSelectionViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    public void Dispose() => Disposables.Dispose();
}