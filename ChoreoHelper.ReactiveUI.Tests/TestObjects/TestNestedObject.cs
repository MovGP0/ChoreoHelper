using ReactiveUI.SourceGenerators;

namespace ReactiveUI.TestObjects;

public sealed partial class TestNestedObject : ReactiveObject
{
    [Reactive] public int _nestedProperty;
}