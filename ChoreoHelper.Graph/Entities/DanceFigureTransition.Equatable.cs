namespace ChoreoHelper.Graph.Entities;

public sealed partial class DanceFigureTransition : IEquatable<DanceFigureTransition>
{
    [Pure]
    public bool Equals(DanceFigureTransition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Source.Equals(other.Source)
               && Target.Equals(other.Target)
               && Distance.Equals(other.Distance);
    }

    [Pure]
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is DanceFigureTransition other && Equals(other);

    [Pure]
    public override int GetHashCode() => HashCode.Combine(Source, Target, Distance);

    [Pure]
    public static bool operator ==(DanceFigureTransition left, DanceFigureTransition right) => Equals(left, right);

    [Pure]
    public static bool operator !=(DanceFigureTransition left, DanceFigureTransition right) => !Equals(left, right);
}