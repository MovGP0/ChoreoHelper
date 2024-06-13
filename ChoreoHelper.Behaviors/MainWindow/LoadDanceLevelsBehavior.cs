using System.Collections.Immutable;
using SliccDB.Serialization;
using ChoreoHelper.Database;
using Splat;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class LoadDanceLevelsBehavior(DatabaseConnection connection) : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var sourceList = new SourceList<LevelSelectionViewModel>();

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
                sourceList.Clear();
                sourceList.AddRange(items);
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