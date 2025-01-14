using ChoreoHelper.Dance;
using ChoreoHelper.Entities;
using ChoreoHelper.Figure;
using ChoreoHelper.LevelSelection;
using ChoreoHelper.OptionalFigureSelection;
using ChoreoHelper.RequiredFigureSelection;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search;

public sealed partial class SearchViewModel:
    ReactiveObject,
    IActivatableViewModel,
    IDisposable,
    IRoutableViewModel
{
    public SearchViewModel()
    {
        HostScreen = null!;

        if (this.IsInDesignMode())
        {
            InitializeDesignTimeData();
        }
    }

    [UsedImplicitly]
    public SearchViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;

        if (this.IsInDesignMode())
        {
            InitializeDesignTimeData();
        }

        this.WhenActivated(this.ActivateBehaviors);
    }

    private void InitializeDesignTimeData()
    {
        Dances.Add(new () { Name = "Waltz", Category = "Standard" });
        Dances.Add(new () { Name = "Tango", Category = "Standard" });
        Dances.Add(new () { Name = "Foxtrot", Category = "Standard" });
        Dances.Add(new () { Name = "Quickstep", Category = "Standard" });
        Dances.Add(new () { Name = "Viennese Waltz", Category = "Standard" });

        for (var i = 0; i < 5; i++)
        {
            RequiredFigures.Add(new RequiredFigureSelectionViewModel());
        }

        for (var i = 0; i < 5; i++)
        {
            OptionalFigures.Add(new OptionalFigureSelectionViewModel());
        }

        Levels.Add(new LevelSelectionViewModel { Level = DanceLevel.Bronze });
        Levels.Add(new LevelSelectionViewModel { Level = DanceLevel.Silver });
        Levels.Add(new LevelSelectionViewModel { Level = DanceLevel.Gold });

        FindChoreography = EnabledCommand.Instance;
    }

    [Reactive]
    private DanceViewModel? _selectedDance;

    [Reactive]
    private string _searchText = string.Empty;

    [Reactive]
    private bool _isStartWithSpecificFigure;

    public ObservableCollectionExtended<DanceViewModel> Dances { get; } = new();

    /// <summary>
    /// All figures of the selected dance.
    /// </summary>
    public ObservableCollectionExtended<FigureViewModel> Figures { get; } = new();

    public ObservableCollectionExtended<RequiredFigureSelectionViewModel> RequiredFigures { get; } = new();

    public ObservableCollectionExtended<RequiredFigureSelectionViewModel> RequiredFiguresFiltered { get; } = new();

    public ObservableCollectionExtended<RequiredFigureSelectionViewModel> SelectedRequiredFigures { get; } = new();

    [Reactive]
    private RequiredFigureSelectionViewModel? _selectedSpecificStartFigure;

    public ObservableCollectionExtended<OptionalFigureSelectionViewModel> OptionalFigures { get; } = new();

    public ObservableCollectionExtended<OptionalFigureSelectionViewModel> OptionalFiguresFiltered { get; } = new();

    public ObservableCollectionExtended<LevelSelectionViewModel> Levels { get; } = new();

    public DanceLevel GetLevels()
    {
        return Levels
            .Where(l => l.IsSelected)
            .Aggregate(DanceLevel.Undefined, (acc, l) => acc | l.Level);
    }

    [Reactive]
    private IReactiveCommand _findChoreography = DisabledCommand.Instance;

    public ViewModelActivator Activator { get; } = new();

    public void Dispose() => Activator.Dispose();

    public string UrlPathSegment => "search";
    public IScreen HostScreen { get; }
}