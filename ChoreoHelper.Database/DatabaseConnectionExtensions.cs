using System.Globalization;
using SliccDB.Core;
using SliccDB.Fluent;
using SliccDB.Serialization;

namespace ChoreoHelper.Database;

public static class DatabaseConnectionExtensions
{
    public static int[,] GetDistanceMatrix(this DatabaseConnection connection, DanceStepNodeInfo[] figures)
    {
        var matrix = new int[figures.Length, figures.Length];
        for (var row = 0; row < figures.Length; row++)
        for (var col = 0; col < figures.Length; col++)
        {
            var fromFigure = figures[col];
            var toFigure = figures[row];
            var distance = connection.GetDistance(fromFigure, toFigure);
            matrix[row, col] = distance;
        }
        return matrix;
    }

    public static int GetDistance(
        this DatabaseConnection connection,
        DanceStepNodeInfo sourceFigure,
        DanceStepNodeInfo targetFigure)
    {
        var relation = (
            from r in connection.Relations()
            where r.SourceHash == sourceFigure.Hash
            where r.TargetHash == targetFigure.Hash
            where r.Properties.Any(p => p.Key == "distance")
            select r)
            .FirstOrDefault();

        if (relation == default)
        {
            return -1;
        }

        var value = relation.Properties.First(p => p.Key == "distance").Value;

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int valueInt))
        {
            return valueInt;
        }

        return -1;
    }

    public static IEnumerable<string> GetDances(this DatabaseConnection connection)
    {
        return connection
            .Nodes()
            .SelectMany(n => n.Properties)
            .Where(p => p.Key == "dance")
            .Select(p => p.Value)
            .Distinct();
    }

    public static IEnumerable<DanceStepNodeInfo> GetFigures(
        this DatabaseConnection connection,
        string dance,
        DanceLevel level = DanceLevel.All)
    {
        return connection
            .Nodes()
            .WhereDanceName(dance)
            .WhereLevel(level)
            .Select(n =>
            {
                var levelProperties = n.Properties
                    .Where(p => p.Key == "level")
                    .Take(1)
                    .ToImmutableArray();

                if (levelProperties.Any())
                {
                    var danceLevel = levelProperties[0].Value switch
                    {
                        "bronze" => DanceLevel.Bronze,
                        "silver" => DanceLevel.Silver,
                        "gold" => DanceLevel.Gold,
                        _ => DanceLevel.Undefined
                    };
                    return new DanceStepNodeInfo(n.Labels.First(), n.Hash, danceLevel);
                }

                return new DanceStepNodeInfo(n.Labels.First(), n.Hash, DanceLevel.Undefined);
            });
    }

    private static IEnumerable<Node> WhereDanceName(this IEnumerable<Node> nodes, string danceName)
    {
        return nodes
            .Where(e => e.Properties.Any(kv =>
                kv.Key == "dance"
                && string.Equals(kv.Value, danceName, StringComparison.InvariantCultureIgnoreCase)));
    }

    private static IEnumerable<Node> WhereLevel(this IEnumerable<Node> nodes, DanceLevel level)
    {
        foreach (Node e1 in nodes)
        {
            if (e1.Properties.Any(e =>
                    e.Key == "level"
                    && ((e.Value == "bronze" && level.IsFlagSet(DanceLevel.Bronze)) ||
                        (e.Value == "silver" && level.IsFlagSet(DanceLevel.Silver)) ||
                        (e.Value == "gold" && level.IsFlagSet(DanceLevel.Gold)) ||
                        (e.Value == "advanced" && level.IsFlagSet(DanceLevel.Advanced)))))
            {
                yield return e1;
            }
        }
    }

    [Pure]
    public static IImmutableSet<DanceLevel> GetDanceLevels(this DatabaseConnection connection)
    {
        HashSet<DanceLevel> levels = new();

        var levelProperties = connection.Nodes
                .SelectMany(node => node.Properties)
                .Where(property => property.Key == "level");

        foreach (var property in levelProperties)
        {
            levels.Add(property.Value switch
            {
                "bronze" => DanceLevel.Bronze,
                "silver" => DanceLevel.Silver,
                "gold" => DanceLevel.Gold,
                "advanced" => DanceLevel.Advanced,
                _ => DanceLevel.Undefined
            });
        }

        return levels.ToImmutableHashSet();
    }
}