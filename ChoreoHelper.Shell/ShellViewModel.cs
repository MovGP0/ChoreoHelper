using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

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
    public ReactiveCommand<Unit, Unit> SaveXmlData { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> GoToTransitionEditor { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> GoToSearch { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> GoToSearchResult { get; set; } = DisabledCommand.Instance;

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