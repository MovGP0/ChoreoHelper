using System.Diagnostics;
using ChoreoHelper.Algorithms;
using ChoreoHelper.Entities;
using ChoreoHelper.Messages;
using Microsoft.Extensions.Logging;
using ChoreoHelper.Search.Extensions;

namespace ChoreoHelper.Search.Algorithms;

public sealed class SearchChoreographies(
    IUnreachableIslandsFinder unreachableIslandsFinder,
    IRouteFinder routeFinder,
    DancesCache dancesCache,
    ILogger<SearchChoreographies> logger) : ISearchChoreographies
{
    public async Task<FoundChoreographies> ExecuteAsync(SearchViewModel viewModel, CancellationToken ct)
    {
        var requiredFigures = viewModel.RequiredFigures
            .Where(rf => rf.IsSelected)
            .Select(rf => new DanceStepNodeInfo(rf.Name, rf.Level))
            .ToArray();

        var optionalFigures = viewModel.OptionalFigures
            .Where(of => of.IsSelected)
            .Select(of => new DanceStepNodeInfo(of.Name, of.Level))
            .ToArray();

        var figures = requiredFigures.Concat(optionalFigures)
            .ToArray();

        var danceName = viewModel.SelectedDance?.Name ?? string.Empty;
        if (danceName == string.Empty)
        {
            logger.LogWarning("No dance selected");
            return new([]);
        }

        var (matrix, sortedFigures) = dancesCache.Items.GetDistanceMatrix(danceName, figures);
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
        if (viewModel is { IsStartWithSpecificFigure: true, SelectedSpecificStartFigure: {} specificStartFigure })
        {
            startNode = Array.FindIndex(sortedFigures, sf => sf.Name == specificStartFigure.Name);
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
        var items = routes
            .Select<Route, DanceStepNodeInfo[]>(route => route
                .VisitedNodes.Reverse().Select(index => sortedFigures[index])
                .ToArray())
            .ToArray();

        return new FoundChoreographies(items);
    }
}