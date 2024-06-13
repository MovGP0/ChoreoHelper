using System.Reactive.Disposables;
using ChoreoHelper.Database;
using DynamicData.Binding;
using ReactiveUI;
using Splat;

namespace ChoreoHelper.ViewModels;

public sealed class ChoreographyViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    private float _rating;
    public float Rating
    {
        get => _rating;
        set => this.RaiseAndSetIfChanged(ref _rating, value);
    }

    public IObservableCollection<DanceStepNodeInfo> Figures { get; }
        = new ObservableCollectionExtended<DanceStepNodeInfo>();

    public ChoreographyViewModel()
    {
        if (this.IsInDesignMode())
        {
            Rating = 5;
            for(var i = 0; i < 5; i++)
            {
                Figures.Add(new DanceStepNodeInfo("Foobar", "01234", DanceLevel.Gold));
            }
        }

        foreach (var behavior in Locator.Current.GetServices<IBehavior<ChoreographyViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    public void Dispose() => Disposables.Dispose();
}