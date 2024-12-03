using ChoreoHelper.Dance;
using ChoreoHelper.Entities;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Search.Behaviors;

public sealed class LoadDancesBehavior(
    ISubscriber<Messages.DataLoadedEvent> dataLoadedSubscriber) : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var dancesList = new SourceList<DanceViewModel>()
            .DisposeWith(disposables);

        dancesList.Connect()
            .Bind(viewModel.Dances)
            .Subscribe()
            .DisposeWith(disposables);

        dataLoadedSubscriber
            .Subscribe(data =>
            {
                viewModel.DancesCollection.Clear();
                viewModel.DancesCollection.AddRange(data.Dances);

                var vms = data.Dances
                    .OrderBy(d => d.Category, StringComparer.CurrentCulture)
                    .ThenBy(d => d.Name, StringComparer.CurrentCulture)
                    .Select(ToViewModel)
                    .ToImmutableArray();

                dancesList.Update(vms);
            })
            .DisposeWith(disposables);
    }

    private static DanceViewModel ToViewModel(ChoreoHelper.Entities.Dance dance)
    {
        var vm = Locator.Current.GetRequiredService<DanceViewModel>();
        vm.Hash = dance.Name;
        vm.Category = dance.Category;
        vm.Name = dance.Name;
        return vm;
    }

    private static DanceViewModel ToViewModel(DanceInfo danceInfo)
    {
        var vm = Locator.Current.GetRequiredService<DanceViewModel>();
        vm.Hash = danceInfo.Hash;
        vm.Category = danceInfo.Category;
        vm.Name = danceInfo.Name;
        return vm;
    }
}