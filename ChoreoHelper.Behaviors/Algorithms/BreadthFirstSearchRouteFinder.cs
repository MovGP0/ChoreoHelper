using System.Collections.Immutable;
using System.Diagnostics;

namespace ChoreoHelper.Behaviors.Algorithms;

public sealed class BreadthFirstSearchRouteFinder : IRouteFinder
{
    public async Task<List<Route>> FindAllRoutesAsync(
        int[,] distanceMatrix,
        ImmutableArray<int> requiredNodes,
        int? startNode,
        int maxDistance,
        CancellationToken cancellationToken = default)
    {
        var frontier = new HashSet<Route>();

        if (startNode is null)
        {
            foreach (var node in requiredNodes)
            {
                frontier.Add(new Route(node));
            }
        }
        else
        {
            frontier.Add(new Route(startNode.Value));
        }

        while (!cancellationToken.IsCancellationRequested
               && frontier.Count > 0
               && !frontier.Any(f => f.HasVisitedAllRequiredNodes(requiredNodes)))
        {
            HashSet<Route> newFrontier = new();
            var minDistance = frontier.Select(r => r.Distance).Min();

            foreach (var route in frontier.OrderBy(e => e.Distance))
            {
                if (route.Distance > minDistance)
                {
                    if (frontier.Count < 10_000)
                    {
                        // schedule for later
                        newFrontier.Add(route);
                    }

                    continue;
                }

                Debug.Assert(route.Distance == minDistance);

                for (var node = 0; node < distanceMatrix.GetLength(0); node++)
                {
                    var distance = distanceMatrix[route.LastVisitedNode, node];
                    if (distance < 0)
                    {
                        // not connected
                        continue;
                    }

                    var penalty = route.VisitedNodes.Count(n => n == node);
                    var newRoute = route.Append(node, route.Distance + distance + penalty);
                    Debug.Assert(newRoute.LastVisitedNode == node);

                    var numberOfRepetitions =
                        newRoute.VisitedNodes.Count()
                        - newRoute.VisitedNodes.Distinct().Count();

                    if (numberOfRepetitions >= 4)
                    {
                        // skip when there are too many repetitions
                        continue;
                    }

                    if (newRoute.Distance <= maxDistance)
                    {
                        newFrontier.Add(newRoute);
                    }
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