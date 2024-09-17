using DynamicData.Binding;

namespace ChoreoHelper.ViewModels;

public sealed class SearchResultViewModel: ReactiveObject, IActivatableViewModel, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    public IObservableCollection<ChoreographyViewModel> Choreographies { get; }
        = new ObservableCollectionExtended<ChoreographyViewModel>();

    public SearchResultViewModel()
    {
        if (this.IsInDesignMode())
        {
            for (var i = 0; i < 5; i++)
            {
                Choreographies.Add(new ChoreographyViewModel());
            }
        }

        foreach(var behavior in Locator.Current.GetServices<IBehavior<SearchResultViewModel>>())
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