using System.Collections.Immutable;
using System.Diagnostics;
using ChoreoHelper.Behaviors.Algorithms;
using ChoreoHelper.Behaviors.Extensions;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;
using ChoreoHelper.Messages;
using DynamicData.Kernel;
using Microsoft.Extensions.Logging;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class FindChoreographyBehavior(
    IDanceFiguresRepository connection,
    ILogger<FindChoreographyBehavior> logger)
    : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var choreographies = new SourceList<ChoreographyViewModel>()
            .DisposeWith(disposables);

        choreographies
            .Connect()
            .Bind(viewModel.Choreographies)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        // at least two required figures are selected
        var obs0 = viewModel.RequiredFigures
            .OnCollectionChanged()
            .Select(_ => Unit.Default);

        var obs1 = MessageBus.Current.Listen<RequiredFigureUpdated>()
            .Do(message =>
            {
                var figure = viewModel.RequiredFigures
                    .FirstOrOptional(rf => rf.Hash == message.Hash);

                if (figure.HasValue)
                {
                    figure.Value.IsSelected = message.IsSelected;
                }
            })
            .Select(_ => Unit.Default);

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

                var danceName = vm.SelectedDance?.Name ?? string.Empty;
                if (danceName == string.Empty)
                {
                    logger.LogWarning("No dance selected");
                    return Array.Empty<DanceStepNodeInfo[]>();
                }

                var (matrix, sortedFigures) = connection.GetDistanceMatrix(danceName, figures);
                var islands = DepthFirstSearchUnreachableIslandsFinder.FindUnreachableIslands(matrix);
                if (islands.Count > 1)
                {
                    logger.LogWarning($"Found {islands.Count} islands");
                }

                var requiredFigureIndex = new int[requiredFigures.Length];
                for (var i = 0; i < requiredFigureIndex.Length; i++)
                {
                    var requiredFigure = requiredFigures[i];
                    requiredFigureIndex[i] = Array.IndexOf(sortedFigures, requiredFigure);
                }

                var nodes = requiredFigureIndex.ToImmutableArray();

                var timeout = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(figures.Length);
                using var timeoutCts = new CancellationTokenSource(timeout);
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

                List<Route> routes = new();
                try
                {
                    routes = await BreadthFirstSearchRouteFinder.FindAllRoutesAsync(
                        matrix,
                        nodes,
                        figures.Length * 2,
                        cts.Token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failure while determining choreography route");
                }

                // convert routes:List<List<int>> to figures:List<List<DanceStepNodeInfo>>
                return routes
                    .Select<Route, DanceStepNodeInfo[]>(route => route
                        .VisitedNodes.Reverse().Select(index => sortedFigures[index])
                        .ToArray())
                    .ToArray();
            })
            .Subscribe(choreographyItems =>
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