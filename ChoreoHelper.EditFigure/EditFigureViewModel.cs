using ChoreoHelper.LevelSelection;
using ChoreoHelper.Restriction;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.EditFigure;

public sealed class EditFigureViewModel : ReactiveObject, IActivatableViewModel
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

    /// <summary>
    /// The hash code of the dance step.
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    public ObservableCollectionExtended<LevelSelectionViewModel> Levels { get; } = new();

    [Reactive]
    public LevelSelectionViewModel? Level { get; set; }

    public ObservableCollectionExtended<RestrictionViewModel> Restrictions { get; } = new();

    [Reactive]
    public RestrictionViewModel? Restriction { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> SaveAndNavigateBack { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> NavigateBack { get; set; } = DisabledCommand.Instance;

    public ViewModelActivator Activator { get; } = new();
}