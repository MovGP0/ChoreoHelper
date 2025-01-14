using ChoreoHelper.LevelSelection;
using ChoreoHelper.Restriction;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.EditFigure;

public sealed partial class EditFigureViewModel : ReactiveObject, IActivatableViewModel
{
    public EditFigureViewModel()
    {
        Levels.AddRange(LevelSelectionViewModelFactory.Create());
        Restrictions.AddRange(RestrictionViewModelFactory.Create());

        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.WhenActivated(disposables =>
        {
            foreach (var behavior in Locator.Current.GetServices<IBehavior<EditFigureViewModel>>())
            {
                behavior.Activate(this, disposables);
            }
        });
    }

    private void InitializeDesignModeData()
    {
        Hash = "Viennese Waltz|Natural Turn";
        Name = "Natural Turn";
        Level = Levels.First();
        Restriction = Restrictions.First();
        SaveAndNavigateBack = EnabledCommand.Instance;
        NavigateBack = EnabledCommand.Instance;
    }

    [Reactive]
    private string _danceName = string.Empty;

    /// <summary>
    /// The hash code of the dance step.
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    [Reactive]
    private string _name = string.Empty;

    public ObservableCollectionExtended<LevelSelectionViewModel> Levels { get; } = new();

    [Reactive]
    private LevelSelectionViewModel? _level;

    public ObservableCollectionExtended<RestrictionViewModel> Restrictions { get; } = new();

    [Reactive]
    private RestrictionViewModel? _restriction;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _saveAndNavigateBack = DisabledCommand.Instance;

    [Reactive]
    private ReactiveCommand<Unit, Unit> _navigateBack = DisabledCommand.Instance;

    public ViewModelActivator Activator { get; } = new();
}