namespace ChoreoHelper.Entities;

public sealed class DistanceComparer : IEqualityComparer<Distance>, IComparer<Distance>
{
    public static DistanceComparer Default { get; } = new(0.0001f);

    private readonly float _epsilon;

    private DistanceComparer(float epsilon)
    {
        _epsilon = epsilon;
    }

    public bool Equals(Distance? x, Distance? y)
    {
        // check for nullability
        if (x is null && y is null)
            return true;
        
        if (x is null || y is null)
            return false;

        // Both are None
        if (x.IsT1 && y.IsT1)
            return true;

        // Both are Unknown
        if (x.IsT2 && y.IsT2)
            return true;

        // One is None/Unknown and the other is not
        if (x.IsT1 || y.IsT1 || x.IsT2 || y.IsT2)
            return false;

        // Both are floats, compare their values
        return Math.Abs(x.AsT0 - y.AsT0) < _epsilon; // Floating point tolerance
    }

    public int GetHashCode(Distance obj)
    {
        if (obj.IsT1)
            return -1; // Use a fixed hash code for None

        if (obj.IsT2)
            return -2; // Use a fixed hash code for Unknown

        return obj.AsT0.GetHashCode();
    }

    public int Compare(Distance? x, Distance? y)
    {
        // check for nullability
        if (x is null && y is null)
            return 0;
        
        if (x is null)
            return -1;
        
        if (y is null)
            return 1;

        // Handle cases where one or both are None
        if (x.IsT1 && y.IsT1)
            return 0; // None is equal to None

        if (x.IsT2 && y.IsT2)
            return 0; // None is equal to None
        
        if (x.IsT1)
            return -1; // None is less than any float

        if (y.IsT1)
            return 1; // Any float is greater than None

        if (x.IsT2)
            return -1;

        if (y.IsT2)
            return 1;

        // Compare float values
        return x.AsT0.CompareTo(y.AsT0);
    }
}