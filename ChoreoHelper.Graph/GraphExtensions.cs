using System.Text;
using ChoreoHelper.Entities;
using ChoreoHelper.Graph.Entities;

namespace ChoreoHelper.Graph;

public static class GraphExtensions
{
    [Pure]
    public static IEnumerable<DanceLevel> GetDanceLevels(
        this UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
    {
        return graph.Vertices
            .Select(v => v.Level)
            .Select(DanceLevelConverter.ToDanceLevel)
            .Distinct();
    }

    [Pure]
    public static IEnumerable<DanceFigure> GetFigures(
        this UndirectedGraph<DanceFigure, DanceFigureTransition> graph,
        string? dance,
        DanceLevel level = DanceLevel.All)
    {
        if (dance is null)
        {
            return Array.Empty<DanceFigure>();
        }

        return graph.Vertices
            .WhereDanceName(dance)
            .WhereLevel(level);
    }

    [Pure]
    private static IEnumerable<DanceFigure> WhereDanceName(this IEnumerable<DanceFigure> nodes, string danceName)
    {
        return nodes
            .Where(e => string.Equals(e.Dance, danceName, StringComparison.InvariantCultureIgnoreCase));
    }

    [Pure]
    private static IEnumerable<DanceFigure> WhereLevel(this IEnumerable<DanceFigure> nodes, DanceLevel level)
    {
        foreach (var e1 in nodes)
        {
            if ((e1.Level == "bronze" && level.IsFlagSet(DanceLevel.Bronze)) ||
                (e1.Level == "silver" && level.IsFlagSet(DanceLevel.Silver)) ||
                (e1.Level == "gold" && level.IsFlagSet(DanceLevel.Gold)) ||
                (e1.Level == "advanced" && level.IsFlagSet(DanceLevel.Advanced)))
            {
                yield return e1;
            }
        }
    }

    [Pure]
    public static IEnumerable<string> GetDances(this UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
    {
        return graph.Vertices
            .Select(n => n.Dance)
            .Distinct();
    }

    [Pure]
    public static int[,] GetDistanceMatrix(this UndirectedGraph<DanceFigure, DanceFigureTransition> graph, DanceFigure[] figures)
    {
        var matrix = new int[figures.Length, figures.Length];

        for (var row = 0; row < figures.Length; row++)
        for (var col = 0; col < figures.Length; col++)
        {
            var fromFigure = figures[row];
            var toFigure = figures[col];
            var distance = graph.Edges
                .Where(v => v.Source == fromFigure && v.Target == toFigure)
                .Select(v => v.Distance)
                .Take(1)
                .ToArray();

            if (distance.Length < 1)
            {
                matrix[row, col] = -1;
            }
            else
            {
                matrix[row, col] = (int)distance[0];
            }
        }

        return matrix;
    }

    private static string PrintMatrix(int[,] matrix)
    {
        var sb = new StringBuilder();
        for (var row = 0; row < matrix.GetLength(0); row++)
        {
            for (var col = 0; col < matrix.GetLength(1); col++)
            {
                char symbol = matrix[row, col] switch
                {
                    -1 => ' ',
                    1 => '\u2591',
                    2 => '\u2593',
                    _ => '\u259E',
                };

                sb.Append(symbol);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}