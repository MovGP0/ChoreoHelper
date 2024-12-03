using ChoreoHelper.Figure;
using ChoreoHelper.RequiredFigureSelection;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadRequiredFiguresBehavior : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var requiredFigures = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Name)
            .DisposeWith(disposables);

        requiredFigures
            .Connect()
            .Bind(viewModel.RequiredFigures)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
            .Select(_ => viewModel)
            .Select(vm => vm.Figures)
            .Subscribe(loadedFigures =>
            {
                foreach (var loadedFigure in loadedFigures)
                {
                    var vm = ToViewModel(loadedFigure);
                    requiredFigures.AddOrUpdate(vm);
                }

                var toDelete = requiredFigures.Keys
                    .Except(loadedFigures.Select(e => e.Name));

                requiredFigures.RemoveKeys(toDelete);
            })
            .DisposeWith(disposables);
    }

    private static IObservable<Unit> Observe(SearchViewModel viewModel)
    {
        return viewModel.Figures
            .OnCollectionChanged()
            .Select(_ => Unit.Default);
    }

    [Pure]
    private static RequiredFigureSelectionViewModel ToViewModel(FigureViewModel loadedFigure)
    {
        var vm = new RequiredFigureSelectionViewModel();
        vm.Name = loadedFigure.Name;
        vm.IsSelected = false;
        vm.Level = loadedFigure.Level;
        return vm;
    }
}