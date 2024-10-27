using System.Reactive;
using System.Reactive.Disposables;
using ChoreoHelper.Editor.Business;
using ChoreoHelper.Editor.ViewModels;
using Microsoft.Win32;
using OneOf;
using OneOf.Types;
using ReactiveUI;

namespace ChoreoHelper.Editor.Behaviors;

public sealed class OpenFileBehavior : IBehavior<MainViewModel>
{
    private readonly XmlDataLoader _xmlDataLoader;

    public OpenFileBehavior(XmlDataLoader xmlDataLoader) => _xmlDataLoader = xmlDataLoader;

    public void Activate(MainViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel.Open = ReactiveCommand
            .Create<Unit>(_ => {})
            .DisposeWith(disposables);

        viewModel.Open
            .Subscribe(_ =>
            {
                if (!SelectXmlFile().TryPickT0(out var filePath, out var _))
                {
                    // user cancelled the selection of the file
                    return;
                }

                var dances = _xmlDataLoader.LoadDances(viewModel.FilePath);
                if (!dances.Any())
                {
                    // the file could not be loaded
                    return;
                }

                // reset view model
                viewModel.FilePath = filePath;
                viewModel.SelectedDance = null;
                viewModel.Dances.Clear();
                viewModel.Figures.Clear();
                viewModel.Transitions = new byte[0, 0];
                viewModel.Dances.AddRange(dances);
            })
            .DisposeWith(disposables);
    }

    private OneOf<string, None> SelectXmlFile()
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