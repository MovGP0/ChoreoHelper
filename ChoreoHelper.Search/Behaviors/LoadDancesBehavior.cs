using System.Collections.Immutable;
using ChoreoHelper.Dance;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadDancesBehavior(IDanceFiguresRepository connection) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var dancesList = new SourceList<DanceViewModel>()
            .DisposeWith(disposables);

        dancesList.Connect()
            .Bind(viewModel.Dances)
            .Subscribe()
            .DisposeWith(disposables);

        Observable
            .Return(true)
            .Select(_ => GetDancesInAlphabeticalOrder())
            .Subscribe(items =>
            {
                var vms = items
                    .Select((i, idx) => ToViewModel(i))
                    .ToImmutableArray();

                dancesList.Update(vms);
            })
            .DisposeWith(disposables);
    }

    private DanceViewModel ToViewModel(DanceInfo danceInfo)
    {
        var vm = Locator.Current.GetRequiredService<DanceViewModel>();
        vm.Hash = danceInfo.Hash;
        vm.Category = danceInfo.Category;
        vm.Name = danceInfo.Name;
        return vm;
    }

    private DanceInfo[] GetDancesInAlphabeticalOrder()
    {
        return connection
            .GetDances()
            .OrderBy(d => d.Category)
            .ThenBy(d => d.Name)
            .ToArray();
    }
}