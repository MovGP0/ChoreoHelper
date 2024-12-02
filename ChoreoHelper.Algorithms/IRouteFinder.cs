using System.Collections.Immutable;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Algorithms;

public interface IRouteFinder
{
    public Task<List<Route>> FindAllRoutesAsync(
        Distance[,] distanceMatrix,
        ImmutableArray<int> requiredNodes,
        int? startNode,
        int maxDistance,
        CancellationToken cancellationToken = default);
}