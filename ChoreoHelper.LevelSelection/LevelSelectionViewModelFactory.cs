using ChoreoHelper.Entities;
using ChoreoHelper.I18N;

namespace ChoreoHelper.LevelSelection;

public static class LevelSelectionViewModelFactory
{
    public static IEnumerable<LevelSelectionViewModel> Create()
    {
        yield return new()
        {
            Name = LevelResources.Undefined,
            Level = DanceLevel.Undefined
        };

        yield return new()
        {
            Name = LevelResources.Bronze,
            Level = DanceLevel.Bronze
        };

        yield return new()
        {
            Name = LevelResources.Silver,
            Level = DanceLevel.Silver
        };

        yield return new()
        {
            Name = LevelResources.Gold,
            Level = DanceLevel.Gold
        };

        yield return new()
        {
            Name = LevelResources.Advanced,
            Level = DanceLevel.Advanced
        };
    }
}