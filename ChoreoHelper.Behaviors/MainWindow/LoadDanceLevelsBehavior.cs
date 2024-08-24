using System.Collections.Immutable;
using ChoreoHelper.Behaviors.Extensions;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadDanceLevelsBehavior(IDanceFiguresRepository connection) : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var sourceList = new SourceList<LevelSelectionViewModel>()
            .DisposeWith(disposables);

        sourceList.Connect()
            .Bind(viewModel.Levels)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        Observable
            .Return(true)
            .Select(_ => connection.GetDanceLevels())
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
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