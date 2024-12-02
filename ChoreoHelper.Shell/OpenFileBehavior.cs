using ChoreoHelper.Gateway;
using ChoreoHelper.Messages;
using Microsoft.Win32;

namespace ChoreoHelper.Shell;

public sealed class OpenFileBehavior(
    XmlDataLoader xmlDataLoader,
    IPublisher<DataLoadedEvent> dataLoadedPublisher) : IBehavior<ShellViewModel>
{
    public void Activate(ShellViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand
            .Create<Unit>(_ => {})
            .DisposeWith(disposables);

        command
            .Subscribe(_ =>
            {
                if (!SelectXmlFile().TryPickT0(out var filePath, out var _))
                {
                    // user cancelled the selection of the file
                    return;
                }

                var dances = xmlDataLoader.LoadDances(filePath);
                if (!dances.Any())
                {
                    // the file could not be loaded
                    return;
                }

                // reset view model
                viewModel.FilePath = filePath;
                dataLoadedPublisher.Publish(new DataLoadedEvent(dances));
            })
            .DisposeWith(disposables);

        viewModel.LoadXmlData = command;
    }

    private static OneOf<string, None> SelectXmlFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select a file",
            Filter = "XML files (*.xml)|*.xml"
        };

        bool? result = openFileDialog.ShowDialog();
        if (result != true)
        {
            return new None();
        }

        return openFileDialog.FileName;
    }
}