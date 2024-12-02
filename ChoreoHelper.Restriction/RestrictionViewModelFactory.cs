using ChoreoHelper.Entities;

namespace ChoreoHelper.Restriction;

public static class RestrictionViewModelFactory
{
    public static IEnumerable<RestrictionViewModel> Create()
    {
        yield return new()
        {
            Description = "Allowed in all classes",
            Restriction = CompetitionRestriction.AllowedInAllClasses
        };

        yield return new()
        {
            Description = "Not allowed in class D, C, B, A",
            Restriction = CompetitionRestriction.NotAllowedInClassA
        };

        yield return new()
        {
            Description = "Not allowed in class D, C, B",
            Restriction = CompetitionRestriction.NotAllowedInClassB
        };

        yield return new()
        {
            Description = "Not allowed in class D, C",
            Restriction = CompetitionRestriction.NotAllowedInClassC
        };

        yield return new()
        {
            Description = "Not allowed in class D",
            Restriction = CompetitionRestriction.NotAllowedInClassD
        };
    }
}