namespace ChoreoHelper.Graph.Entities;

public sealed partial class Dance : IEquatable<Dance>
{
    public bool Equals(Dance? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Category == other.Category
               && Name == other.Name;
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Dance other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Category, Name);

    public static bool operator ==(Dance? left, Dance? right) => Equals(left, right);

    public static bool operator !=(Dance? left, Dance? right) => !Equals(left, right);
}