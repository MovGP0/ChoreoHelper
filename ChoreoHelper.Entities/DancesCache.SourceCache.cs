namespace ChoreoHelper.Entities;

public sealed partial class DancesCache : ISourceCache<Dance, string>
{
    /// <inheritdoc cref="ISourceCache{TObject,TKey}.Edit(Action{ISourceUpdater{TObject,TKey}})"/>
    public void Edit(Action<ISourceUpdater<Dance, string>> updateAction) => _dances.Edit(updateAction);

    /// <inheritdoc cref="ISourceCache{TObject,TKey}.KeySelector"/>
    public Func<Dance, string> KeySelector => _dances.KeySelector;
}