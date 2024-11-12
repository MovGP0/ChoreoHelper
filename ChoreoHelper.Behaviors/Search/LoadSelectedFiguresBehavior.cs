using ChoreoHelper.Messages;
using DynamicData.Kernel;

namespace ChoreoHelper.Behaviors.Search;

public sealed class LoadSelectedFiguresBehavior : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var selectedFigures = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Hash)
            .DisposeWith(disposables);

        selectedFigures.Connect()
            .Bind(viewModel.SelectedRequiredFigures)
            .Subscribe()
            .DisposeWith(disposables);

        MessageBus.Current.Listen<RequiredFigureUpdated>()
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

                var selectedFigure = new RequiredFigureSelectionViewModel();
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
        SearchViewModel viewModel,
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