namespace ChoreoHelper.Entities;

public static class DanceLevelExtensions
{
    [Pure]
    public static bool IsFlagSet(
        this DanceLevel level,
        DanceLevel flagToCheck)
        => (level & flagToCheck) == flagToCheck;
}