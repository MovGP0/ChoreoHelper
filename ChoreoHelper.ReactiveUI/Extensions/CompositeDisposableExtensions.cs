using System.Reactive.Disposables;

namespace ReactiveUI.Extensions;

public static class CompositeDisposableExtensions
{
    public static void Add(this CompositeDisposable disposables, Action action) => disposables.Add(Disposable.Create(action));
}