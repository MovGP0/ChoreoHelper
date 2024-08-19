namespace ChoreoHelper.Behaviors.Algorithms;

public sealed class Route : IEquatable<Route>
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

    public bool Equals(Route? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _visitedNodes.SequenceEqual(other._visitedNodes)
               && Distance == other.Distance;
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Route other && Equals(other);

    private int? _hash;
    public override int GetHashCode() => _hash ??= CalculateHash();

    private int CalculateHash()
    {
        var hash = new HashCode();
        foreach (var item in _visitedNodes)
        {
            hash.Add(item);
        }

        hash.Add(Distance);
        return hash.ToHashCode();
    }
    
    public static bool operator ==(Route? left, Route? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Route? left, Route? right)
    {
        return !Equals(left, right);
    }
}