namespace ChoreoHelper.Entities;

public sealed partial class DancesCache
{
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => _dances.Dispose();
}