namespace ChoreoHelper.Algorithms;

public sealed partial class Route : IEquatable<Route>
{
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

    public static bool operator ==(Route? left, Route? right) => Equals(left, right);

    public static bool operator !=(Route? left, Route? right) => !Equals(left, right);
}