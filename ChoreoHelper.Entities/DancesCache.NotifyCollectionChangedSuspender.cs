using DynamicData.Binding;

namespace ChoreoHelper.Entities;

public sealed partial class DancesCache : INotifyCollectionChangedSuspender
{
    /// <inheritdoc cref="INotifyCollectionChangedSuspender.SuspendCount()"/>
    public IDisposable SuspendCount() => _dances.SuspendCount();

    /// <inheritdoc cref="INotifyCollectionChangedSuspender.SuspendNotifications()"/>
    public IDisposable SuspendNotifications() => _dances.SuspendNotifications();
}