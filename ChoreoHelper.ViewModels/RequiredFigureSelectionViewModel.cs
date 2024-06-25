namespace ChoreoHelper.ViewModels;

public sealed class RequiredFigureSelectionViewModel : ReactiveObject, IDisposable
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
    public string Name { get; set; }

    public RequiredFigureSelectionViewModel()
    {
        if (this.IsInDesignMode())
        {
            Name = "Dance Figure Name";
            IsSelected = true;
        }

        foreach (var behavior in Locator.Current.GetServices<IBehavior<RequiredFigureSelectionViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    public void Dispose() => Disposables.Dispose();
}