using System.Globalization;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.I18N;
using ChoreoHelper.InternetTime;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using NodaTime;

namespace ChoreoHelper.Shell.Behaviors;

public sealed class SaveDancesBehavior(
    DancesCache dancesCache,
    IXmlDataSaver saver,
    IClock clock,
    ILogger<SaveDancesBehavior> logger) : IBehavior<ShellViewModel>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Activate(ShellViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.CreateFromTask<Unit, Unit>((_, ct) => SaveDancesWithLockingAsync(ct), CanActivate())
            .DisposeWith(disposables);

        viewModel.SaveXmlData = command;
        Disposable.Create(() => viewModel.SaveXmlData = DisabledCommand.Instance).DisposeWith(disposables);
    }

    private IObservable<bool> CanActivate()
    {
        return dancesCache.Connect()
            .Select(_ => dancesCache.Count > 0);
    }

    private async Task<Unit> SaveDancesWithLockingAsync(CancellationToken cancellationToken)
    {
        if (!await _semaphore.WaitAsync(TimeSpan.FromMilliseconds(150), cancellationToken))
        {
            logger.LogWarning("Semaphore could not be acquired");
            return Unit.Default;
        }

        try
        {
            await SaveDancesAsync(cancellationToken);
            return Unit.Default;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task SaveDancesAsync(CancellationToken cancellationToken)
    {
        var now = clock.GetCurrentInstant();
        var utc = now.InUtc();
        var internetTime = SwatchInternetTimeCalculator.Calculate(now);
        SaveFileDialog saveFileDialog = new()
        {
            Filter = ShellResources.SaveFileDialog_Filter,
            Title = ShellResources.SaveFileDialog_Title,
            FileName = $"{utc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}_{internetTime}_{ShellResources.SaveFileDialog_FileName}.xml"
        };

        var result = saveFileDialog.ShowDialog();
        if (result == true)
        {
            var dances = dancesCache.Items;
            await saver.SaveAsync(saveFileDialog.FileName, dances, cancellationToken);
        }
    }
}