namespace ChoreoHelper.Database;

public static class DanceLevelExtensions
{
    public static bool IsFlagSet(this DanceLevel level, DanceLevel flagToCheck)
        => (level & flagToCheck) == flagToCheck;
}