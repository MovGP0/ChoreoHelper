namespace ReactiveUI.TestObjects;

public interface IBatchingReactiveObject
{
    event EventHandler<BatchPropertyChangedEventArgs>? PropertiesChanged;
}