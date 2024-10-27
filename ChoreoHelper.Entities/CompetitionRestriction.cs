namespace ChoreoHelper.Entities;

/// <summary>
/// Defines if a dance figure has a restriction of a specific dance class in competitions.
/// A restriction also applies to all lower levels.
/// </summary>
[Flags]
public enum CompetitionRestriction
{
    NotAllowedInClassD = 0b0001,
    NotAllowedInClassC = 0b0011,
    NotAllowedInClassB = 0b0111,
    NotAllowedInClassA = 0b1111,
    AllowedInAllClasses = 0b0000
}