namespace ChoreoHelper.ViewModels;

public sealed class LevelSelectionViewModel: ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public DanceLevel Level { get; set; }

    public int LevelSort => (int)Level;

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public bool IsSelected { get; set; } = true;

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