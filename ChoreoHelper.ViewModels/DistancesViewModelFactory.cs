namespace ChoreoHelper.ViewModels;

public static class DistancesViewModelFactory
{
    public static IEnumerable<DistanceViewModel> Create()
    {
        yield return new()
        {
            Description = DistanceResources.Undefined,
            Distance = new Unknown()
        };

        yield return new()
        {
            Description = DistanceResources.NotReachable,
            Distance = new None()
        };

        yield return new()
        {
            Description = DistanceResources.ReachableWithoutModification,
            Distance = 1
        };

        yield return new()
        {
            Description = DistanceResources.ReachableWithModification,
            Distance = 2
        };
    }
}