using ChoreoHelper.Entities;
using DynamicData.Kernel;

namespace ChoreoHelper.Search.Extensions;

public static class DanceCollectionExtensions
{
    public static (Distance[,] array, DanceStepNodeInfo[] figures) GetDistanceMatrix(
        this IReadOnlyCollection<Entities.Dance> dances,
        string danceName,
        DanceStepNodeInfo[] figures)
    {
        // Ensure that there are figures and the dance name is valid
        if (figures.Length == 0 || string.IsNullOrEmpty(danceName))
        {
            return (new Distance[0, 0], figures);
        }

        // Filter dances to find the matching one by name
        var matchingDance = dances.FirstOrDefault(d => d.Name.Equals(danceName, StringComparison.OrdinalIgnoreCase));
        if (matchingDance == null)
        {
            return (new Distance[0, 0], figures);
        }

        // Get hash set of figure hashes for quick lookup
        var figureHashes = new HashSet<string>(figures.Select(f => f.Name));

        // Find dance figures matching the hashes in the input
        var matchingFigures = matchingDance.Figures
            .Where(figure => figureHashes.Contains(figure.Name))
            .ToArray();

        if (matchingFigures.Length == 0)
        {
            return (new Distance[0, 0], figures);
        }

        // Generate the distance matrix
        var matrix = GetDistanceMatrix(matchingDance, matchingFigures);

        // Sort the resulting figures to match the input order
        var sortedFigures = (
                from matchingFigure in matchingFigures
                join figure in figures on matchingFigure.Name equals figure.Name
                select figure)
            .ToArray();

        return (matrix, sortedFigures);
    }

    private static Distance[,] GetDistanceMatrix(Entities.Dance dance, DanceFigure[] figures)
    {
        var matrix = new Distance[figures.Length, figures.Length];

        for (var row = 0; row < figures.Length; row++)
        for (var col = 0; col < figures.Length; col++)
        {
            var fromFigure = figures[row];
            var toFigure = figures[col];
            matrix[row, col] = GetDistance(dance, fromFigure, toFigure);
        }

        return matrix;
    }

    private static Distance GetDistance(Entities.Dance dance, DanceFigure fromFigure, DanceFigure toFigure)
    {
        var result = dance.Transitions
            .FirstOrOptional(t => string.Equals(t.Source.Name, fromFigure.Name) && string.Equals(t.Target.Name, toFigure.Name));

        return result.HasValue
            ? result.Value.Distance
            : new None();
    }
}