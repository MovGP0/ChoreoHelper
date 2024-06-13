namespace ChoreoHelper.Database;

[Flags]
public enum DanceLevel
{
    Undefined = 0b_0000,
    Bronze = 0b_0001,
    Silver = 0b_0010,
    Gold = 0b_0100,
    Advanced = 0b_1000,
    BronzeToSilver = Bronze | Silver,
    BronzeToGold = Bronze | Silver | Gold,
    All = Bronze | Silver | Gold | Advanced
}