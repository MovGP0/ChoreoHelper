using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    IUnreachableIslandsFinder unreachableIslandsFinder,
    IRouteFinder routeFinder,
    ILogger<FindChoreographyBehavior> logger)
    : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
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

        var obs2 = viewModel.WhenAnyValue(
                vm => vm.IsStartWithSpecificFigure,
                vm => vm.SelectedSpecificStartFigure)
            .Select(_ => Unit.Default);

        IObservable<bool> canExecute =
            obs0.Merge(obs1).Merge(obs2)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Select(vm => RequiredFiguresAreSelected(vm)
                          && StartWithSpecificFigureIsValid(vm));

        var command = ReactiveCommand
            .Create(DoNothing, canExecute)
            .DisposeWith(disposables);

        command
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Select(_ => viewModel)
            .SelectMany(async (SearchViewModel vm, CancellationToken ct) =>
            {
                var requiredFigures = vm.RequiredFigures
                    .Where(rf => rf.IsSelected)
                    .Select(rf => new DanceStepNodeInfo(rf.Name, rf.Hash, rf.Level))
                    .ToArray();

                var optionalFigures = vm.OptionalFigures
                    .Where(of => of.IsSelected)
                    .Select(of => new DanceStepNodeInfo(of.Name, of.Hash, of.Level))
                    .ToArray();

                var figures = requiredFigures.Concat(optionalFigures)
                    .ToArray();

                var danceName = vm.SelectedDance?.Name ?? string.Empty;
                if (danceName == string.Empty)
                {
                    logger.LogWarning("No dance selected");
                    return [];
                }

                var (matrix, sortedFigures) = connection.GetDistanceMatrix(danceName, figures);
                var islands = unreachableIslandsFinder.FindUnreachableIslands(matrix);
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

                int? startNode = null;
                if (viewModel.IsStartWithSpecificFigure && vm.SelectedSpecificStartFigure is {} specificStartFigure)
                {
                    startNode = Array.FindIndex(sortedFigures, sf => sf.Hash == specificStartFigure.Hash);
                    Debug.Assert(startNode >= 0);
                }

                var timeout = Debugger.IsAttached ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(figures.Length);
                using var timeoutCts = new CancellationTokenSource(timeout);
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

                List<Route> routes = new();
                try
                {
                    routes = await routeFinder.FindAllRoutesAsync(
                        matrix,
                        nodes,
                        startNode,
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
            .Subscribe(items => MessageBus.Current.SendMessage(new FoundChoreographies(items)))
            .DisposeWith(disposables);

        viewModel.FindChoreography = command;
        return;

        Unit DoNothing() => Unit.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool StartWithSpecificFigureIsValid(SearchViewModel vm)
    {
        return !vm.IsStartWithSpecificFigure
               || vm.SelectedSpecificStartFigure is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool RequiredFiguresAreSelected(SearchViewModel vm)
        => vm.RequiredFigures.Count(rf => rf.IsSelected) >= 2;
}