using ChoreoHelper.Messages;
using DynamicData.Kernel;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadSelectedFiguresBehavior : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var selectedFigures = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Hash)
            .DisposeWith(disposables);

        selectedFigures.Connect()
            .Bind(viewModel.SelectedRequiredFigures)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        MessageBus.Current.Listen<RequiredFigureUpdated>()
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe(message =>
            {
                var figureOption = viewModel.RequiredFigures
                    .FirstOrOptional(rf => rf.Hash == message.Hash);

                if (!figureOption.HasValue)
                {
                    return;
                }

                var figure = figureOption.Value;
                figure.IsSelected = message.IsSelected;

                var selectedFigure = Locator.Current.GetRequiredService<RequiredFigureSelectionViewModel>();
                using (selectedFigure.SuppressChangeNotifications())
                {
                    selectedFigure.Hash = figure.Hash;
                    selectedFigure.Name = figure.Name;
                }

                UpdateSelectedFigures(
                    viewModel,
                    message.IsSelected,
                    selectedFigures,
                    selectedFigure);
            })
            .DisposeWith(disposables);
    }

    private static void UpdateSelectedFigures(
        MainWindowViewModel viewModel,
        bool isSelected,
        SourceCache<RequiredFigureSelectionViewModel, string> selectedFigures,
        RequiredFigureSelectionViewModel selectedFigure)
    {
        if (isSelected)
        {
            selectedFigures.AddOrUpdate(selectedFigure);
            return;
        }
        selectedFigures.Remove(selectedFigure);

        if (viewModel.SelectedSpecificStartFigure != null && viewModel.SelectedSpecificStartFigure.Hash == selectedFigure.Hash)
        {
            viewModel.SelectedSpecificStartFigure = null;
        }
    }
}