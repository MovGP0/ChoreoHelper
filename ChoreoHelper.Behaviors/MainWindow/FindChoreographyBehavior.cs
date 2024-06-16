using System.Collections.Immutable;
using System.Diagnostics;
using ChoreoHelper.Behaviors.Algorithms;
using ChoreoHelper.Database;
using ChoreoHelper.Messages;
using Microsoft.Extensions.Logging;
using SliccDB.Serialization;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class FindChoreographyBehavior(
    DatabaseConnection connection,
    ILogger<FindChoreographyBehavior> logger)
    : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var choreographies = new SourceList<ChoreographyViewModel>();

        choreographies
            .Connect()
            .Bind(viewModel.Choreographies)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        // at least two required figures are selected
        var obs0 = viewModel.WhenAnyValue(vm => vm.RequiredFigures).Select(_ => Unit.Default);
        var obs1 = MessageBus.Current.Listen<RequiredFigureUpdated>().Select(_ => Unit.Default);

        IObservable<bool> canExecute =
            obs0.Merge(obs1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Select(vm => vm.RequiredFigures.Count(rf => rf.IsSelected) >= 2);

        var command = ReactiveCommand
            .Create(DoNothing, canExecute)
            .DisposeWith(disposables);

        command
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Select(_ => viewModel)
            .SelectMany(async (MainWindowViewModel vm, CancellationToken ct) =>
            {
                var requiredFigures = vm.RequiredFigures
                    .Where(rf => rf.IsSelected)
                    .Select(f => new DanceStepNodeInfo(f.Name, f.Hash, f.Level))
                    .ToArray();

                var optionalFigures = vm.OptionalFigures
                    .Where(rf => rf.IsSelected)
                    .Select(f => new DanceStepNodeInfo(f.Name, f.Hash, f.Level))
                    .ToArray();

                var figures = requiredFigures.Concat(optionalFigures)
                    .ToArray();

                var matrix = connection.GetDistanceMatrix(figures);
                var islands = DepthFirstSearchUnreachableIslandsFinder.FindUnreachableIslands(matrix);
                if (islands.Count > 1)
                {
                    logger.LogWarning($"Found {islands.Count} islands");
                }

                var requiredFigureIndex = new int[requiredFigures.Length];
                for (var i = 0; i < requiredFigureIndex.Length; i++)
                {
                    requiredFigureIndex[i] = i;
                }

                var nodes = requiredFigureIndex.ToImmutableArray();

                var timeout = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(figures.Length);
                using var timeoutCts = new CancellationTokenSource(timeout);
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);
                var routes = await BreadthFirstSearchRouteFinder.FindAllRoutesAsync(
                    matrix,
                    nodes,
                    figures.Length * 2,
                    cts.Token);

                // convert routes:List<List<int>> to figures:List<List<DanceStepNodeInfo>>
                return routes
                    .Select<Route, DanceStepNodeInfo[]>(route => route
                        .VisitedNodes.Select(index => figures[index])
                        .ToArray())
                    .ToArray();
            })
            .Subscribe<DanceStepNodeInfo[][]>(choreographyItems =>
            {
                choreographies.Clear();

                var viewModels = choreographyItems
                    .Select(ToChoreographyViewModel)
                    .ToImmutableArray();

                choreographies.AddRange(viewModels);
            })
            .DisposeWith(disposables);

        viewModel.FindChoreography = command;
        return;

        Unit DoNothing() => Unit.Default;
    }

    [Pure]
    private static ChoreographyViewModel ToChoreographyViewModel(DanceStepNodeInfo[] item)
    {
        ChoreographyViewModel choreographyViewModel = new()
        {
            Rating = 1f / item.Length * 10f
        };
        choreographyViewModel.Figures.AddRange(item);
        return choreographyViewModel;
    }
}