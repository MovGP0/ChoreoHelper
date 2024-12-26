using ChoreoHelper.Gateway;
using ChoreoHelper.I18N;
using ChoreoHelper.Messages;
using Microsoft.Win32;

namespace ChoreoHelper.Shell.Behaviors;

public sealed class OpenFileBehavior(
    IXmlDataLoader xmlDataLoader,
    IPublisher<DataLoadedEvent> dataLoadedPublisher)
    : IBehavior<ShellViewModel>
{
    public void Activate(ShellViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand
            .CreateFromTask<Unit, Unit>((_, ct) => OpenFileAsync(viewModel, ct))
            .DisposeWith(disposables);

        viewModel.LoadXmlData = command;
    }

    private async Task<Unit> OpenFileAsync(ShellViewModel viewModel, CancellationToken ct)
    {
        if (!SelectXmlFile().TryPickT0(out var filePath, out var _))
        {
            // user cancelled the selection of the file
            return Unit.Default;
        }

        var dances = await xmlDataLoader.LoadDancesAsync(filePath, ct);
        if (!dances.Any())
        {
            // the file could not be loaded
            return Unit.Default;
        }

        // reset view model
        viewModel.FilePath = filePath;
        dataLoadedPublisher.Publish(new DataLoadedEvent(dances));
        return Unit.Default;
    }

    private static OneOf<string, None> SelectXmlFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = ShellResources.OpenFileDialog_Title,
            Filter = ShellResources.OpenFileDialog_Filter
        };

        bool? result = openFileDialog.ShowDialog();
        if (result != true)
        {
            return new None();
        }

        return openFileDialog.FileName;
    }
}