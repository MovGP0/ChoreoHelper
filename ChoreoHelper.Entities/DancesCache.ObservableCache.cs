using DynamicData.Kernel;

namespace ChoreoHelper.Entities;

public sealed partial class DancesCache
{
    /// <inheritdoc cref="IObservableCache{TObject,TKey}.Count"/>
    public int Count => _dances.Count;

    /// <inheritdoc cref="IObservableCache{TObject,TKey}.Items"/>
    public IReadOnlyList<Dance> Items => _dances.Items;

    /// <inheritdoc cref="IObservableCache{TObject,TKey}.Keys"/>
    public IReadOnlyList<string> Keys => _dances.Keys;

    /// <inheritdoc cref="IObservableCache{TObject,TKey}.KeyValues"/>
    public IReadOnlyDictionary<string, Dance> KeyValues => _dances.KeyValues;

    /// <inheritdoc cref="IObservableCache{TObject,TKey}.Lookup(TKey)"/>
    public Optional<Dance> Lookup(string key) => _dances.Lookup(key);
}