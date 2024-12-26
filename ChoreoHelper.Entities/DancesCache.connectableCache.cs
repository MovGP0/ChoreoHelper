namespace ChoreoHelper.Entities;

public sealed partial class DancesCache
{
    /// <inheritdoc cref="IConnectableCache{TObject,TKey}.Connect(Func{TObject,bool}?, bool)"/>
    public IObservable<IChangeSet<Dance, string>> Connect(Func<Dance, bool>? predicate = null, bool suppressEmptyChangeSets = true) => _dances.Connect(predicate, suppressEmptyChangeSets);

    /// <inheritdoc cref="IConnectableCache{TObject,TKey}.Connect"/>
    public IObservable<IChangeSet<Dance, string>> Preview(Func<Dance, bool>? predicate = null) => _dances.Preview(predicate);

    /// <inheritdoc cref="IConnectableCache{TObject,TKey}.Watch(TKey)"/>
    public IObservable<Change<Dance, string>> Watch(string key) => _dances.Watch(key);

    /// <inheritdoc cref="IConnectableCache{TObject,TKey}.CountChanged"/>
    public IObservable<int> CountChanged => _dances.CountChanged;
}