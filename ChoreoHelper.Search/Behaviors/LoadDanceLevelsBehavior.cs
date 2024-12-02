using System.Collections.Immutable;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.LevelSelection;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadDanceLevelsBehavior(IDanceFiguresRepository connection) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var sourceList = new SourceList<LevelSelectionViewModel>()
            .DisposeWith(disposables);

        sourceList.Connect()
            .Bind(viewModel.Levels)
            .Subscribe()
            .DisposeWith(disposables);

        Observable
            .Return(true)
            .Select(_ => connection.GetDanceLevels())
            .Select(ToViewModels)
            .Subscribe(items =>
            {
                sourceList.Update(items);
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