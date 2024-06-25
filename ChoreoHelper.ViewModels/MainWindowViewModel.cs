using DynamicData.Binding;

namespace ChoreoHelper.ViewModels;

public sealed class MainWindowViewModel: ReactiveObject, IActivatableViewModel, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public bool IsDrawerOpen { get; set; }

    [Reactive]
    public string? SelectedDance { get; set; }

    [Reactive]
    public string SearchText { get; set; }

    public IObservableCollection<string> Dances { get; }
        = new ObservableCollectionExtended<string>();

    public IObservableCollection<RequiredFigureSelectionViewModel> RequiredFigures { get; }
        = new ObservableCollectionExtended<RequiredFigureSelectionViewModel>();

    public IObservableCollection<OptionalFigureSelectionViewModel> OptionalFigures { get; }
        = new ObservableCollectionExtended<OptionalFigureSelectionViewModel>();

    public IObservableCollection<LevelSelectionViewModel> Levels { get; }
        = new ObservableCollectionExtended<LevelSelectionViewModel>();

    public DanceLevel GetLevels()
    {
        return Levels
            .Where(l => l.IsSelected)
            .Aggregate(DanceLevel.Undefined, (acc, l) => acc | l.Level);
    }

    [Reactive]
    public IReactiveCommand FindChoreography { get; set; }

    public IObservableCollection<ChoreographyViewModel> Choreographies { get; }
        = new ObservableCollectionExtended<ChoreographyViewModel>();

    public MainWindowViewModel()
    {
        if (this.IsInDesignMode())
        {
            Dances.Add("Waltz");
            Dances.Add("Tango");
            Dances.Add("Foxtrot");
            Dances.Add("Quickstep");
            Dances.Add("Viennese Waltz");

            for (var i = 0; i < 5; i++)
            {
                RequiredFigures.Add(new RequiredFigureSelectionViewModel());
            }

            for (var i = 0; i < 5; i++)
            {
                OptionalFigures.Add(new OptionalFigureSelectionViewModel());
            }

            FindChoreography = EnabledCommand.Instance;

            for (var i = 0; i < 5; i++)
            {
                Choreographies.Add(new ChoreographyViewModel());
            }
        }

        foreach(var behavior in Locator.Current.GetServices<IBehavior<MainWindowViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    public ViewModelActivator Activator { get; } = new();

    public void Dispose()
    {
        Disposables.Dispose();
        Activator.Dispose();
    }
}