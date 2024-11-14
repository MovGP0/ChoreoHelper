using DynamicData.Binding;

namespace ChoreoHelper.ViewModels;

public sealed class SearchViewModel: ReactiveObject, IActivatableViewModel, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    public SearchViewModel()
    {
        if (this.IsInDesignMode())
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
        }

        foreach(var behavior in Locator.Current.GetServices<IBehavior<SearchViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
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

    public void Dispose()
    {
        Disposables.Dispose();
        Activator.Dispose();
    }
}