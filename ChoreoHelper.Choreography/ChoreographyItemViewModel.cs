using System.Windows.Media;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Choreography;

public sealed partial class ChoreographyItemViewModel : ReactiveObject, IActivatableViewModel
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
    private string _name = string.Empty;

    [Reactive]
    private DanceLevel _level = DanceLevel.All;

    [Reactive]
    private Brush _color = Brushes.Transparent;

    public ViewModelActivator Activator { get; } = new();
}