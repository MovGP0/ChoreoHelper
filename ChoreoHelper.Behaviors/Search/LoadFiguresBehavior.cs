using ChoreoHelper.Behaviors.Extensions;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;

namespace ChoreoHelper.Behaviors.Search;

public sealed class LoadFiguresBehavior(IDanceFiguresRepository connection) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var figures = new SourceCache<FigureViewModel, string>(vm => vm.Hash)
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

                return connection
                    .GetFigures(vm.SelectedDance.Name, DanceLevel.All)
                    .Select(ToViewModel)
                    .ToArray();
            })
            .Subscribe(fs =>
            {
                figures.Update(fs);
            })
            .DisposeWith(disposables);
    }

    [Pure]
    private static FigureViewModel ToViewModel(DanceStepNodeInfo loadedFigure)
    {
        return new FigureViewModel
        {
            Hash = loadedFigure.Hash,
            Name = loadedFigure.Name,
            Level = loadedFigure.Level
        };
    }
}