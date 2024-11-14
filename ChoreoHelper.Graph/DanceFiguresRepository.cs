using System.Collections.Immutable;
using ChoreoHelper.Entities;
using ChoreoHelper.Gateway;

namespace ChoreoHelper.Graph;

public sealed class DanceFiguresRepository : IDanceFiguresRepository
{
    private OneOf<UndirectedGraph<DanceFigure, DanceFigureTransition>, None> Graph { get; set; } = new None();

    public (OneOf<float, None>[,] array, DanceStepNodeInfo[] figures) GetDistanceMatrix(string dance, DanceStepNodeInfo[] figures)
    {
        var task = EnsureGraphIsLoadedAsync();
        task.Wait();

        if (figures.Length == 0 || !Graph.TryPickT0(out var g, out _))
        {
            return new(new OneOf<float, None>[0, 0], figures);
        }

        var hashSet = new HashSet<string>();
        foreach (var figure in figures)
        {
            hashSet.Add(figure.Hash);
        }

        var danceFigures = g.GetFigures(dance, DanceLevel.All)
            .Where(f => hashSet.Contains(GetHash(f)))
            .ToArray();

        var matrix = g.GetDistanceMatrix(danceFigures);
        var sortedFigures = (
            from df in danceFigures
            join f in figures on GetHash(df) equals f.Hash
            select f)
            .ToArray();

        return new(matrix, sortedFigures);
    }

    public IEnumerable<DanceInfo> GetDances()
    {
        var task = EnsureGraphIsLoadedAsync();
        task.Wait();

        return Graph.TryPickT0(out var g, out _)
            ? g.GetDances()
            : Array.Empty<DanceInfo>();
    }

    public IEnumerable<DanceStepNodeInfo> GetFigures(string? dance, DanceLevel level = DanceLevel.All)
    {
        var task = EnsureGraphIsLoadedAsync();
        task.Wait();

        return Graph.TryPickT0(out var g, out _)
            ? g.GetFigures(dance, level).Select(ToDanceStepNodeInfo)
            : [];
    }

    [Pure]
    public IImmutableSet<DanceLevel> GetDanceLevels()
    {
        var task = EnsureGraphIsLoadedAsync();
        task.Wait();

        return Graph.TryPickT0(out var g, out _)
            ? g.GetDanceLevels().ToImmutableHashSet()
            : Array.Empty<DanceLevel>().ToImmutableHashSet();
    }

    private async Task<YesOrNo> EnsureGraphIsLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (Graph.IsT0)
        {
            return new YesOrNo.Yes();
        }

        var reader = new XmlDocumentReader();
        const string filePath = "dance transitions.xml";
        var graph = await reader.ParseXmlDocumentAsync(filePath, cancellationToken);
        if (graph.TryPickT0(out var g, out var _))
        {
            Graph = g;
        }
        return new YesOrNo.No();
    }

    [Pure]
    private static DanceStepNodeInfo ToDanceStepNodeInfo(DanceFigure figure)
    {
        var hash = GetHash(figure);
        var danceLevel = figure.Level;
        return new(figure.Name, hash, danceLevel);
    }

    [Pure]
    private static string GetHash(DanceFigure figure)
    {
        return HashCode.Combine(figure.Name, figure.Level)
            .GetHashCode()
            .ToString(CultureInfo.InvariantCulture);
    }
}