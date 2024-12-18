﻿using ChoreoHelper.Dance;
using ChoreoHelper.Entities;
using ChoreoHelper.Figure;
using ChoreoHelper.Gateway;
using ChoreoHelper.LevelSelection;
using ChoreoHelper.OptionalFigureSelection;
using ChoreoHelper.RequiredFigureSelection;
using ChoreoHelper.Search.Extensions;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search;

public sealed class SearchViewModel:
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
    public DanceViewModel? SelectedDance { get; set; }

    [Reactive]
    public string SearchText { get; set; } = string.Empty;

    [Reactive]
    public bool IsStartWithSpecificFigure { get; set; }

    public IObservableCollection<DanceViewModel> Dances { get; }
        = new ObservableCollectionExtended<DanceViewModel>();

    /// <summary>
    /// All figures of the selected dance.
    /// </summary>
    public IObservableCollection<FigureViewModel> Figures { get; }
        = new ObservableCollectionExtended<FigureViewModel>();

    public IObservableCollection<RequiredFigureSelectionViewModel> RequiredFigures { get; }
        = new ObservableCollectionExtended<RequiredFigureSelectionViewModel>();

    public IObservableCollection<RequiredFigureSelectionViewModel> RequiredFiguresFiltered { get; }
        = new ObservableCollectionExtended<RequiredFigureSelectionViewModel>();

    public IObservableCollection<RequiredFigureSelectionViewModel> SelectedRequiredFigures { get; }
        = new ObservableCollectionExtended<RequiredFigureSelectionViewModel>();

    [Reactive]
    public RequiredFigureSelectionViewModel? SelectedSpecificStartFigure { get; set; }

    public IObservableCollection<OptionalFigureSelectionViewModel> OptionalFigures { get; }
        = new ObservableCollectionExtended<OptionalFigureSelectionViewModel>();

    public IObservableCollection<OptionalFigureSelectionViewModel> OptionalFiguresFiltered { get; }
        = new ObservableCollectionExtended<OptionalFigureSelectionViewModel>();

    public IObservableCollection<LevelSelectionViewModel> Levels { get; }
        = new ObservableCollectionExtended<LevelSelectionViewModel>();

    public DanceLevel GetLevels()
    {
        return Levels
            .Where(l => l.IsSelected)
            .Aggregate(DanceLevel.Undefined, (acc, l) => acc | l.Level);
    }

    [Reactive] public IReactiveCommand FindChoreography { get; set; } = DisabledCommand.Instance;

    public ViewModelActivator Activator { get; } = new();

    public void Dispose() => Activator.Dispose();

    public string UrlPathSegment => "search";
    public IScreen HostScreen { get; }

    public ICollection<Entities.Dance> DancesCollection { get; } = new List<Entities.Dance>(); 

    public (Distance[,] array, DanceStepNodeInfo[] figures) GetDistanceMatrix(string danceName, DanceStepNodeInfo[] figures)
    {
        return DancesCollection.GetDistanceMatrix(danceName, figures);
    }
}