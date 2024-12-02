namespace ChoreoHelper.Shell;

public sealed class ShellViewModel : ReactiveObject, IScreen, IActivatableViewModel
{
    public RoutingState Router { get; } = new();

    /// <summary>
    /// The path to the XML data source.
    /// </summary>
    [Reactive]
    public string FilePath { get; set; } = string.Empty;

    [Reactive]
    public ReactiveCommand<Unit, Unit> LoadXmlData { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> GoToTransitionEditor { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> GoToSearch { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> GoToSearchResult { get; set; } = DisabledCommand.Instance;

    public ShellViewModel()
    {
        this.WhenActivated(disposables =>
        {
            foreach (var behavior in Locator.Current.GetServices<IBehavior<ShellViewModel>>())
            {
                behavior.Activate(this, disposables);
            }
        });
    }

    public ViewModelActivator Activator { get; } = new();
}