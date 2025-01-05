using System.Windows.Media;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Choreography;

public sealed class ChoreographyItemViewModel : IActivatableViewModel
{
    public ChoreographyItemViewModel()
    {
        this.WhenActivated(disposables =>
        {
            foreach (var behavior in Locator.Current.GetServices<IBehavior<ChoreographyItemViewModel>>())
            {
                behavior.Activate(this, disposables);
            }
        });
    }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public DanceLevel Level { get; set; }

    [Reactive]
    public Brush Color { get; set; } = Brushes.Transparent;

    public ViewModelActivator Activator { get; } = new();
}