namespace ReactiveUI.TestObjects;

public sealed class TestObserver<T> : IObserver<T>
{
    public List<T> ReceivedValues { get; } = new();
    public List<Exception> ReceivedErrors { get; } = new();
    public bool IsCompleted { get; private set; }

    public void OnCompleted() => IsCompleted = true;
    public void OnError(Exception error) => ReceivedErrors.Add(error);
    public void OnNext(T value) => ReceivedValues.Add(value);
}
