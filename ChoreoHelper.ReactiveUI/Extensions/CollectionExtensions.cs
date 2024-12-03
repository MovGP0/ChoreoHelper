namespace ReactiveUI.Extensions;

public static class CollectionExtensions
{
    public static void AddRange<TCollection, TItem>(this TCollection collection, IEnumerable<TItem> items)
        where TCollection : ICollection<TItem>
    {
        if (collection is List<TItem> list)
        {
            list.AddRange(items);
            return;
        }

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}