using System.Diagnostics;
using System.Globalization;
using ChoreoHelper.Entities;
using DynamicData.Binding;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Choreography;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed partial class ChoreographyViewModel:
    ReactiveObject,
    IActivatableViewModel,
    IDisposable
{
    [Reactive]
    private float _rating;

    public IObservableCollection<ChoreographyItemViewModel> Figures { get; }
        = new ObservableCollectionExtended<ChoreographyItemViewModel>();

    [Reactive]
    private IReactiveCommand _copy = DisabledCommand.Instance;

    public ChoreographyViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignTimeData();
        }

        this.WhenActivated(this.ActivateBehaviors);
    }

    private void InitializeDesignTimeData()
    {
        Copy = EnabledCommand.Instance;
        Rating = 5;
        for (var i = 0; i < 5; i++)
        {
            var vm = new ChoreographyItemViewModel
            {
                Name = $"Figure {i}",
                Level = DanceLevel.Gold
            };
            Figures.Add(vm);
        }
    }

    private string DebuggerDisplay
    {
        get
        {
            var rating = Rating.ToString(CultureInfo.InvariantCulture);
            var figures = string.Join(", ", Figures.Select(f => f.Name));
            return $"Rating = {rating}; Figures = [{figures}]";
        }
    }

    public ViewModelActivator Activator { get; } = new();

    public void Dispose() => Activator.Dispose();
}