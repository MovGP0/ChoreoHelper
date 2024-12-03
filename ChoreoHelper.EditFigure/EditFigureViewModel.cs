using ChoreoHelper.LevelSelection;
using ChoreoHelper.Restriction;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.EditFigure;

public sealed class EditFigureViewModel : ReactiveObject
{
    public EditFigureViewModel()
    {
        Levels.AddRange(LevelSelectionViewModelFactory.Create());
        Restrictions.AddRange(RestrictionViewModelFactory.Create());

        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }
    }

    private void InitializeDesignModeData()
    {
        Hash = "43523984";
        Name = "Some Figure Name";
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

    public IObservableCollection<LevelSelectionViewModel> Levels { get; }
        = new ObservableCollectionExtended<LevelSelectionViewModel>();

    [Reactive]
    public LevelSelectionViewModel? Level { get; set; }

    public IObservableCollection<RestrictionViewModel> Restrictions { get; }
        = new ObservableCollectionExtended<RestrictionViewModel>();

    [Reactive]
    public RestrictionViewModel? Restriction { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> SaveAndNavigateBack { get; set; } = DisabledCommand.Instance;

    [Reactive]
    public ReactiveCommand<Unit, Unit> NavigateBack { get; set; } = DisabledCommand.Instance;
}