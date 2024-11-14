using System.Collections.Specialized;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.Extensions;

public static class ObservableCollectionExtensions
{
    public static IObservable<NotifyCollectionChangedEventArgs> OnCollectionChanged<T>(this IObservableCollection<T> collection)
    {
        return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            handler => collection.CollectionChanged += handler,
            handler => collection.CollectionChanged -= handler)
            .Select(e => e.EventArgs);
    }
}