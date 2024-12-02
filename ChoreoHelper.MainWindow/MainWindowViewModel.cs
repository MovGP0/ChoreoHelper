using ChoreoHelper.Search;
using ChoreoHelper.SearchResult;

namespace ChoreoHelper.MainWindow;

public sealed class MainWindowViewModel: ReactiveObject, IActivatableViewModel, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    [Reactive]
    public bool IsDrawerOpen { get; set; }

    public MainWindowViewModel()
    {
        foreach(var behavior in Locator.Current.GetServices<IBehavior<MainWindowViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    public SearchViewModel SearchViewModel { get; } = new();
    public SearchResultViewModel SearchResultViewModel { get; } = new();

    public ViewModelActivator Activator { get; } = new();

    public void Dispose()
    {
        Disposables.Dispose();
        Activator.Dispose();
    }
}