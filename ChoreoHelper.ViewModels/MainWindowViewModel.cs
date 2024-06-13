using System.Reactive.Disposables;
using ChoreoHelper.Database;
using DynamicData.Binding;
using ReactiveUI;
using Splat;

namespace ChoreoHelper.ViewModels;

public sealed class MainWindowViewModel: ReactiveObject, IActivatableViewModel, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    private string? _selectedDance;
    public string? SelectedDance
    {
        get => _selectedDance;
        set => this.RaiseAndSetIfChanged(ref _selectedDance, value);
    }

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

    private IReactiveCommand _findChoreography = DisabledCommand.Instance;

    public IReactiveCommand FindChoreography
    {
        get => _findChoreography;
        set => this.RaiseAndSetIfChanged(ref _findChoreography, value);
    }

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