namespace ChoreoHelper.Entities;

public sealed class DistanceComparer : IEqualityComparer<OneOf<float, None>>, IComparer<OneOf<float, None>>
{
    public static DistanceComparer Default { get; } = new(0.0001f);

    private readonly float _epsilon;
    private DistanceComparer(float epsilon)
    {
        _epsilon = epsilon;
    }
    
    public bool Equals(OneOf<float, None> x, OneOf<float, None> y)
    {
        // Both are None
        if (x.IsT1 && y.IsT1)
            return true;

        // One is None and the other is not
        if (x.IsT1 || y.IsT1)
            return false;

        // Both are floats, compare their values
        return Math.Abs(x.AsT0 - y.AsT0) < _epsilon; // Floating point tolerance
    }

    public int GetHashCode(OneOf<float, None> obj)
    {
        if (obj.IsT1)
            return 0; // Use a fixed hash code for None

        return obj.AsT0.GetHashCode();
    }

    public int Compare(OneOf<float, None> x, OneOf<float, None> y)
    {
        // Handle cases where one or both are None
        if (x.IsT1 && y.IsT1)
            return 0; // None is equal to None

        if (x.IsT1)
            return -1; // None is less than any float

        if (y.IsT1)
            return 1; // Any float is greater than None

        // Compare float values
        return x.AsT0.CompareTo(y.AsT0);
    }
}