namespace ChoreoHelper.Graph.Entities;

public sealed partial class DanceFigure : IEquatable<DanceFigure>
{
    [Pure]
    public bool Equals(DanceFigure? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Dance == other.Dance
               && Name == other.Name
               && Level == other.Level;
    }

    [Pure]
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is DanceFigure other && Equals(other);

    [Pure]
    public override int GetHashCode() => HashCode.Combine(Dance, Name, Level);

    [Pure]
    public static bool operator ==(DanceFigure left, DanceFigure right) => Equals(left, right);

    [Pure]
    public static bool operator !=(DanceFigure left, DanceFigure right) => !Equals(left, right);
}