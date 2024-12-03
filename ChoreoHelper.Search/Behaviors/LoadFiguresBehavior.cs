using ChoreoHelper.Entities;
using ChoreoHelper.Figure;
using DynamicData.Kernel;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadFiguresBehavior(
    ISubscriber<Messages.DataLoadedEvent> dataLoadedSubscriber) : IBehavior<SearchViewModel>
{
    private ICollection<Entities.Dance> Dances { get; } = new List<Entities.Dance>();
    
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

                var dance = Dances.FirstOrOptional(dance => dance.Name == vm.SelectedDance.Name);
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

        dataLoadedSubscriber
            .Subscribe(data =>
            {
                Dances.Clear();
                Dances.AddRange(data.Dances);
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