using System.Diagnostics;
using System.Globalization;
using ChoreoHelper.Entities;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Choreography;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class ChoreographyViewModel:
    ReactiveObject,
    IActivatableViewModel,
    IDisposable
{
    [Reactive]
    public float Rating { get; set; }

    public IObservableCollection<DanceStepNodeInfo> Figures { get; }
        = new ObservableCollectionExtended<DanceStepNodeInfo>();

    [Reactive]
    public IReactiveCommand Copy { get; set; } = DisabledCommand.Instance;

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
            Figures.Add(new DanceStepNodeInfo("Foobar", DanceLevel.Gold));
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