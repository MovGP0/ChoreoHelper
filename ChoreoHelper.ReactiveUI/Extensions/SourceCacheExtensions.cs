using DynamicData;

namespace ReactiveUI.Extensions;

public static class SourceCacheExtensions
{
    public static void Update<TObject, TKey>(
        this SourceCache<TObject, TKey> cache,
        IList<TObject> objects)
        where TObject : notnull
        where TKey : notnull
    {
        Func<TObject, TKey> keySelector = cache.KeySelector;

        var keys = objects.Select(keySelector).ToArray();
        var existingKeys = cache.Items.Select(cache.KeySelector).ToHashSet();
        var toRemove = existingKeys.Except(keys).ToArray();
        var toAdd = keys
            .Except(existingKeys)
            .Select(k => objects.First(o => keySelector(o).Equals(k)))
            .ToArray();

        using (cache.SuspendNotifications())
        {
            cache.Remove(toRemove);
            cache.AddOrUpdate(toAdd);
        }
    }
}