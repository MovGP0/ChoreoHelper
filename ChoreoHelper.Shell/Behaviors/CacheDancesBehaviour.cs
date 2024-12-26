using ChoreoHelper.Entities;
using ChoreoHelper.Messages;

namespace ChoreoHelper.Shell.Behaviors;

public sealed class CacheDancesBehavior(
    ISubscriber<DataLoadedEvent> dataLoadedPublisher,
    DancesCache dancesCache)
    : IBehavior<ShellViewModel>
{
    public void Activate(ShellViewModel viewModel, CompositeDisposable disposables)
    {
        dataLoadedPublisher
            .Subscribe(dataLoadedEvent =>
            {
                using (dancesCache.SuspendNotifications())
                {
                    dancesCache.Clear();
                    dancesCache.AddOrUpdate(dataLoadedEvent.Dances);
                }
            })
            .DisposeWith(disposables);
    }
}