using ChoreoHelper.Messages;
using ChoreoHelper.RequiredFigureSelection;
using DynamicData.Kernel;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadSelectedFiguresBehavior(
    ISubscriber<RequiredFigureUpdated> requiredFigureUpdatedSubscriber)
    : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var selectedFigures = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Name)
            .DisposeWith(disposables);

        selectedFigures.Connect()
            .Bind(viewModel.SelectedRequiredFigures)
            .Subscribe()
            .DisposeWith(disposables);

        viewModel
            .WhenAnyValue(e => e.SelectedDance)
            .Subscribe(_ =>
            {
                viewModel.IsStartWithSpecificFigure = false;
                viewModel.SelectedSpecificStartFigure = null;
                selectedFigures.Clear();
            })
            .DisposeWith(disposables);

        requiredFigureUpdatedSubscriber
            .Subscribe(message =>
            {
                var figureOption = viewModel.RequiredFigures
                    .FirstOrOptional(rf => rf.Name == message.Name);

                if (!figureOption.HasValue)
                {
                    return;
                }

                var figure = figureOption.Value;
                figure.IsSelected = message.IsSelected;

                var selectedFigure = new RequiredFigureSelectionViewModel();
                using (selectedFigure.SuppressChangeNotifications())
                {
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

        if (viewModel.SelectedSpecificStartFigure != null && viewModel.SelectedSpecificStartFigure.Name == selectedFigure.Name)
        {
            viewModel.SelectedSpecificStartFigure = null;
        }
    }
}