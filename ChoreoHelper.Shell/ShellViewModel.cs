using ReactiveUI.Extensions;

namespace ChoreoHelper.Shell;

public sealed partial class ShellViewModel : ReactiveObject, IScreen, IActivatableViewModel
{
    public RoutingState Router { get; } = new();

    /// <summary>
    /// The path to the XML data source.
    /// </summary>
    [Reactive]
    private string _filePath = string.Empty;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _loadXmlData = DisabledCommand.Instance;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _saveXmlData = DisabledCommand.Instance;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _goToTransitionEditor = DisabledCommand.Instance;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _goToSearch = DisabledCommand.Instance;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _goToSearchResult = DisabledCommand.Instance;

    public ShellViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.WhenActivated(this.ActivateBehaviors);
    }

    private void InitializeDesignModeData()
    {
        LoadXmlData = EnabledCommand.Instance;
        GoToTransitionEditor = EnabledCommand.Instance;
        GoToSearch = EnabledCommand.Instance;
        GoToSearchResult = EnabledCommand.Instance;
    }

    public ViewModelActivator Activator { get; } = new();
}