using ChoreoHelper.Entities;

namespace ChoreoHelper.Graph;

public static class DanceLevelConverter
{
    [Pure]
    public static DanceLevel ToDanceLevel(string level)
    {
        return level switch
        {
            "bronze" => DanceLevel.Bronze,
            "silver" => DanceLevel.Silver,
            "gold" => DanceLevel.Gold,
            "advanced" => DanceLevel.Advanced,
            _ => DanceLevel.All
        };
    }
}