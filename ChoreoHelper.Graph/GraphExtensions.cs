using System.Text;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Graph;

public static class GraphExtensions
{
    [Pure]
    public static IEnumerable<DanceLevel> GetDanceLevels(
        this UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
    {
        return graph.Vertices
            .Select(v => v.Level)
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
    public static IEnumerable<DanceInfo> GetDances(this UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
    {
        return graph
            .Vertices
            .Select(e => e.Dance)
            .Select(ToDanceInfo)
            .Distinct();
    }

    private static DanceInfo ToDanceInfo(Dance dance)
    {
        return new DanceInfo(dance.Category, dance.Name, dance.Name);
    }

    [Pure]
    private static IEnumerable<DanceFigure> WhereDanceName(this IEnumerable<DanceFigure> nodes, string danceName)
    {
        return nodes
            .Where(e => string.Equals(e.Dance.Name, danceName, StringComparison.InvariantCultureIgnoreCase));
    }

    [Pure]
    private static IEnumerable<DanceFigure> WhereLevel(this IEnumerable<DanceFigure> danceFigures, DanceLevel level)
    {
        foreach (var danceFigure in danceFigures)
        {
            if ((danceFigure.Level == DanceLevel.Bronze && level.IsFlagSet(DanceLevel.Bronze)) ||
                (danceFigure.Level == DanceLevel.Silver && level.IsFlagSet(DanceLevel.Silver)) ||
                (danceFigure.Level == DanceLevel.Gold && level.IsFlagSet(DanceLevel.Gold)) ||
                (danceFigure.Level == DanceLevel.Advanced && level.IsFlagSet(DanceLevel.Advanced)))
            {
                yield return danceFigure;
            }
        }
    }

    [Pure]
    public static Distance[,] GetDistanceMatrix(this UndirectedGraph<DanceFigure, DanceFigureTransition> graph, DanceFigure[] figures)
    {
        var matrix = new Distance[figures.Length, figures.Length];

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
                matrix[row, col] = new None();
            }
            else
            {
                matrix[row, col] = distance[0];
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