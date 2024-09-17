using System.Collections.Immutable;

namespace ChoreoHelper.Behaviors.Algorithms;

public interface IRouteFinder
{
    public Task<List<Route>> FindAllRoutesAsync(
        int[,] distanceMatrix,
        ImmutableArray<int> requiredNodes,
        int? startNode,
        int maxDistance,
        CancellationToken cancellationToken = default);
}