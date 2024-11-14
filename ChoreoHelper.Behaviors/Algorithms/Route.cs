namespace ChoreoHelper.Behaviors.Algorithms;

public sealed partial class Route
{
    public int Distance { get; }

    private readonly Stack<int> _visitedNodes = new();
    public IEnumerable<int> VisitedNodes => _visitedNodes;

    public int LastVisitedNode => _visitedNodes.Peek();

    public Route(int startNode)
    {
        Distance = 0;
        _visitedNodes.Push(startNode);
    }

    private Route(int distance, IEnumerable<int> nodes)
    {
        Distance = distance;
        foreach (var node in nodes)
        {
            _visitedNodes.Push(node);
        }
    }

    [Pure]
    public Route Append(int node, int distance) => new(distance, VisitedNodes.Reverse().Concat([node]));

    [Pure]
    public bool HasVisitedAllRequiredNodes(
        IEnumerable<int> requiredNodes)
        => requiredNodes.All(VisitedNodes.Contains);
}