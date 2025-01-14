using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using ReactiveMarbles.ObservableEvents;

namespace ReactiveUI.TestObjects;

public abstract class BatchingReactiveObject : ReactiveObject, IBatchingReactiveObject, IDisposable
{
    /// <summary>
    /// Stores a list of the names of changed properties.
    /// </summary>
    private readonly HashSet<string> _changedProperties = new();

    private readonly IDisposable _propertyChangeSubscription;

    protected BatchingReactiveObject()
    {
        _propertyChangeSubscription = this.Events().PropertyChanged
            .Subscribe(HandlePropertyChanged);
    }

    private void HandlePropertyChanged(PropertyChangedEventArgs args)
    {
        if (args.PropertyName is not {} propertyName) return;

        if (_changeNotificationsEnabled)
        {
            _changedProperties.Add(propertyName);
        }
        else
        {
            OnBatchPropertyChanged([propertyName]);
        }
    }

    private readonly Lock _lock = new();
    private bool _changeNotificationsEnabled;

    new public IDisposable DelayChangeNotifications()
    {
        var disposables = new CompositeDisposable();

        lock (_lock)
        {
            _changeNotificationsEnabled = true;
            base.DelayChangeNotifications().DisposeWith(disposables);
        }

        Disposable.Create(NotifyPropertiesChanged).DisposeWith(disposables);
        return disposables;

        void NotifyPropertiesChanged()
        {
            _changeNotificationsEnabled = false;
            if (_changedProperties.Count == 0)
            {
                return;
            }

            OnBatchPropertyChanged([.._changedProperties]);
            _changedProperties.Clear();
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void OnBatchPropertyChanged(ImmutableArray<string> changedProperties)
    {
        var args = new BatchPropertyChangedEventArgs(changedProperties);
        OnPropertiesChanged(this, args);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertiesChanged(object? sender, BatchPropertyChangedEventArgs args)
    {
        PropertiesChanged?.Invoke(sender, args);
    }

    public event EventHandler<BatchPropertyChangedEventArgs>? PropertiesChanged;

    public virtual void Dispose() => _propertyChangeSubscription.Dispose();
}