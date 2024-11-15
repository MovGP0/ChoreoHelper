using OneOf.Types;

namespace ChoreoHelper.ViewModels;

public static class DistancesViewModelFactory
{
    public static IEnumerable<DistanceViewModel> Create()
    {
        yield return new()
        {
            Description = "Not reachable",
            Distance = new None()
        };

        yield return new()
        {
            Description = "Reachable without modification",
            Distance = 1
        };

        yield return new()
        {
            Description = "Reachable with modification",
            Distance = 2
        };
    }
}