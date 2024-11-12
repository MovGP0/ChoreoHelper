using DynamicData.Binding;

namespace ChoreoHelper.ViewModels;

public sealed class EditFigureViewModel : ReactiveObject
{
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
}