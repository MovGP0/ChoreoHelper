namespace ChoreoHelper.ViewModels;

public static class LevelSelectionViewModelFactory
{
    public static IEnumerable<LevelSelectionViewModel> Create()
    {
        yield return new()
        {
            Name = "Undefined",
            Level = DanceLevel.Undefined
        };

        yield return new()
        {
            Name = "Bronze",
            Level = DanceLevel.Bronze
        };

        yield return new()
        {
            Name = "Silver",
            Level = DanceLevel.Silver
        };

        yield return new()
        {
            Name = "Gold",
            Level = DanceLevel.Gold
        };

        yield return new()
        {
            Name = "Advanced",
            Level = DanceLevel.Advanced
        };
    }
}