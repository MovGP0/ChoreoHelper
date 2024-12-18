using DynamicData;

namespace ReactiveUI.Extensions;

public static class SourceListExtensions
{
    public static void Update<TObject>(
        this SourceList<TObject> sourceList,
        IList<TObject> objects)
        where TObject : notnull
    {
        var keys = objects.ToArray();
        var existingKeys = sourceList.Items.ToHashSet();
        var toRemove = existingKeys.Except(keys).ToArray();
        var toAdd = keys
            .Except(existingKeys)
            .ToArray();

        sourceList.RemoveMany(toRemove);
        sourceList.AddRange(toAdd);
    }
}