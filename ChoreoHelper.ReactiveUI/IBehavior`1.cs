using System.Reactive.Disposables;

namespace ReactiveUI;

public interface IBehavior<in T>
    where T : class
{
    void Activate(T viewModel, CompositeDisposable disposables);
}