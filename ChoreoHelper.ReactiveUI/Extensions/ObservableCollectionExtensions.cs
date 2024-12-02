using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData.Binding;
using ReactiveMarbles.ObservableEvents;

namespace ReactiveUI.Extensions;

public static class ObservableCollectionExtensions
{
    [Pure]
    public static IObservable<NotifyCollectionChangedEventArgs> OnCollectionChanged<T>(this IObservableCollection<T> collection)
    {
        return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => collection.CollectionChanged += handler,
                handler => collection.CollectionChanged -= handler)
            .Select(e => e.EventArgs);
    }

    [Pure]
    public static IObservable<Unit> ObservePropertyChanges<T>(this ObservableCollection<T> collection)
        where T : INotifyPropertyChanged
    {
        var subject = new Subject<Unit>();
        var itemSubscriptions = new Dictionary<int, IDisposable>();

        IDisposable collectionChangedSubscription = collection.Events().CollectionChanged
            .Subscribe(changeEvent =>
            {
                switch (changeEvent.Action)
                {
                    case NotifyCollectionChangedAction.Add when changeEvent.NewItems is {} newItems:
                        AddSubscriptions(newItems.OfType<T>(), itemSubscriptions, subject);
                        break;

                    case NotifyCollectionChangedAction.Remove when changeEvent.OldItems is {} oldItems:
                        RemoveSubscription(oldItems.OfType<T>(), itemSubscriptions);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        if (changeEvent.OldItems is { } oldItems1)
                        {
                            RemoveSubscription(oldItems1.OfType<T>(), itemSubscriptions);
                        }
                        if (changeEvent.NewItems is { } newItems1)
                        {
                            AddSubscriptions(newItems1.OfType<T>(), itemSubscriptions, subject);
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        if (changeEvent.OldItems is { } oldItems2)
                        {
                            RemoveSubscription(oldItems2.OfType<T>(), itemSubscriptions);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

        // When the observable is disposed, clean up all subscriptions.
        var disposable = Disposable.Create(() =>
        {
            foreach (var disposable in itemSubscriptions.Values)
            {
                disposable.Dispose();
            }

            itemSubscriptions.Clear();
            subject.OnCompleted();
            subject.Dispose();
        });

        return Observable.Create<Unit>(observer => new CompositeDisposable(
            subject.Subscribe(observer),
            collectionChangedSubscription,
            disposable));
    }

    private static void AddSubscriptions<T>(IEnumerable<T> nis, Dictionary<int, IDisposable> itemSubscriptions, Subject<Unit> subject)
        where T : INotifyPropertyChanged
    {
        foreach (T newItem in nis)
        {
            var itemHash = newItem.GetHashCode();
            if (!itemSubscriptions.ContainsKey(itemHash))
            {
                var propertyChangedSubscription = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => newItem.PropertyChanged += h,
                        h => newItem.PropertyChanged -= h)
                    .Subscribe(_ =>
                    {
                        subject.OnNext(Unit.Default);
                    });

                itemSubscriptions[itemHash] = propertyChangedSubscription;
            }
        }
    }

    private static void RemoveSubscription<T>(IEnumerable<T> items, IDictionary<int, IDisposable> itemSubscriptions)
        where T : INotifyPropertyChanged
    {
        foreach (T oldItem in items)
        {
            var itemHash = oldItem.GetHashCode();
            if (!itemSubscriptions.TryGetValue(itemHash, out var oldItemSubscription))
            {
                continue;
            }

            oldItemSubscription.Dispose();
            itemSubscriptions.Remove(itemHash);
        }
    }
}