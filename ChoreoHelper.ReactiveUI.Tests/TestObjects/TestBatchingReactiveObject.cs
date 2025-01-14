using System.Reactive.Disposables;
using ReactiveUI.SourceGenerators;

namespace ReactiveUI.TestObjects;

public sealed partial class TestBatchingReactiveObject : BatchingReactiveObject, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public TestBatchingReactiveObject()
    {
        this.ObserveChildPropertyChanges(d => d.NestedObject1).DisposeWith(_disposables);
        this.ObserveChildPropertyChanges(d => d.NestedObject2).DisposeWith(_disposables);
    }

    [Reactive] public int _someProperty;
    [Reactive] public string _anotherProperty = string.Empty;
    [Reactive] public TestNestedObject _nestedObject1 = new();
    [Reactive] public TestNestedObject _nestedObject2 = new();

    public void Dispose() => _disposables.Dispose();
}