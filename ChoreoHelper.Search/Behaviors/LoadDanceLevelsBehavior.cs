using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.LevelSelection;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadDanceLevelsBehavior(
    ISubscriber<Messages.DataLoadedEvent> dataLoadedSubscriber) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var sourceList = new SourceList<LevelSelectionViewModel>()
            .DisposeWith(disposables);

        sourceList.Connect()
            .Bind(viewModel.Levels)
            .Subscribe()
            .DisposeWith(disposables);

        dataLoadedSubscriber
            .Subscribe(args =>
            {
                var levels = (
                    from dance in args.Dances
                    from figure in dance.Figures
                    select figure.Level)
                    .Distinct()
                    .Select(ToViewModel)
                    .ToImmutableArray();

                sourceList.Update(levels);
            })
            .DisposeWith(disposables);
    }

    private static List<LevelSelectionViewModel> ToViewModels(IImmutableSet<DanceLevel> items)
        => items.Select(ToViewModel).ToList();

    private static LevelSelectionViewModel ToViewModel(DanceLevel item)
    {
        var vm = Locator.Current.GetRequiredService<LevelSelectionViewModel>();
        vm.Level = item;
        return vm;
    }
}