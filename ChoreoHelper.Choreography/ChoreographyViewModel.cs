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
    IDisposable,
    IRoutableViewModel
{
    [Reactive]
    public float Rating { get; set; }

    public IObservableCollection<DanceStepNodeInfo> Figures { get; }
        = new ObservableCollectionExtended<DanceStepNodeInfo>();

    [Reactive]
    public IReactiveCommand Copy { get; set; } = DisabledCommand.Instance;

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Itself)]
    public ChoreographyViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        this.WhenActivated(ActivateBehaviors);
    }

    public ChoreographyViewModel()
    {
        HostScreen = null!;

        if (this.IsInDesignMode())
        {
            InitializeDesignTimeData();
        }

        this.WhenActivated(ActivateBehaviors);
    }

    private void InitializeDesignTimeData()
    {
        Rating = 5;
        for (var i = 0; i < 5; i++)
        {
            Figures.Add(new DanceStepNodeInfo("Foobar", "01234", DanceLevel.Gold));
        }
    }

    private void ActivateBehaviors(CompositeDisposable disposables)
    {
        foreach (var behavior in Locator.Current.GetServices<IBehavior<ChoreographyViewModel>>())
        {
            behavior.Activate(this, disposables);
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

    public string UrlPathSegment => "choreography";
    public IScreen HostScreen { get; }
}