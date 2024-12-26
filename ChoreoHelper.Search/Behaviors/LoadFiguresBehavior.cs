using ChoreoHelper.Entities;
using ChoreoHelper.Figure;
using DynamicData.Kernel;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadFiguresBehavior(DancesCache dancesCache) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var figures = new SourceCache<FigureViewModel, string>(vm => vm.Name)
            .DisposeWith(disposables);

        figures.Connect()
            .Bind(viewModel.Figures)
            .Subscribe()
            .DisposeWith(disposables);

        viewModel
            .WhenAnyValue(vm => vm.SelectedDance)
            .Select(_ => viewModel)
            .Select(vm =>
            {
                if (vm.SelectedDance is null)
                {
                    return [];
                }

                var dance = dancesCache.Items.FirstOrOptional(dance => dance.Name == vm.SelectedDance.Name);
                if (dance.HasValue)
                {
                    return dance.Value.Figures
                        .Select(ToViewModel)
                        .ToArray();
                }

                return [];
            })
            .Subscribe(fs =>
            {
                figures.Update(fs);
            })
            .DisposeWith(disposables);
    }

    [Pure]
    private static FigureViewModel ToViewModel(DanceFigure loadedFigure)
    {
        return new FigureViewModel
        {
            Name = loadedFigure.Name,
            Level = loadedFigure.Level
        };
    }
}