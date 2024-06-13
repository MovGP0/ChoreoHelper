using System.Collections.Immutable;

namespace ChoreoHelper.Behaviors.Algorithms;

public static class BreadthFirstSearchRouteFinder
{
    public static async Task<List<Route>> FindAllRoutesAsync(
        int[,] distanceMatrix,
        ImmutableArray<int> requiredNodes,
        int maxDistance,
        CancellationToken cancellationToken = default)
    {
        var frontier = new List<Route>();
        foreach (var startNode in requiredNodes)
        {
            frontier.Add(new Route(startNode));
        }

        while (!cancellationToken.IsCancellationRequested
               && frontier.Count > 0
               && !frontier.Any(f => f.HasVisitedAllRequiredNodes(requiredNodes)))
        {
            List<Route> newFrontier = new();

            foreach (var node in distanceMatrix)
            foreach (var route in frontier)
            {
                var distance = distanceMatrix[route.LastVisitedNode, node];
                if (distance < 0)
                {
                    // not connected
                    continue;
                }

                var penalty = route.VisitedNodes.Count(n => n == node);
                var newRoute = route.Append(node, distance + penalty);
                if (newRoute.Distance <= maxDistance)
                {
                    newFrontier.Add(newRoute);
                }
            }

            frontier = newFrontier;
        }

        await Task.CompletedTask;
        return frontier
            .Where(route => route.HasVisitedAllRequiredNodes(requiredNodes))
            .ToList();
    }
}