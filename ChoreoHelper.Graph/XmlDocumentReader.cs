using System.Collections.Immutable;
using ChoreoHelper.Graph.Entities;

namespace ChoreoHelper.Graph;

public sealed class XmlDocumentReader
{
    [Pure]
    public async Task<OneOf<UndirectedGraph<DanceFigure, DanceFigureTransition>, Error>> ParseXmlDocumentAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return new Error();
        }

        XDocument document;
        await using(var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
        {
            document = await XDocument.LoadAsync(fileStream, LoadOptions.None, cancellationToken);
        }

        var rootElement = document.Root;
        if (rootElement is null)
        {
            return new Error();
        }

        var dances = new List<Dance>();
        foreach (var element in rootElement.Elements().Where(e => e.Name.LocalName == nameof(Dance).ToLowerInvariant()))
        {
            var danceOrError = Dance.FromXml(element);
            if (danceOrError.IsT0)
            {
                dances.Add(danceOrError.AsT0);
            }
        }

        var graph = new UndirectedGraph<DanceFigure, DanceFigureTransition>();
        foreach (var element in rootElement.Elements().Where(e => e.Name.LocalName == nameof(DanceFigure).ToLowerInvariant()))
        {
            var figureOrError = DanceFigure.FromXml(element, dances);
            if (figureOrError.IsT0)
            {
                graph.AddVertex(figureOrError.AsT0);
            }
        }

        var allVertices = graph.Vertices.ToImmutableList();
        foreach (var element in rootElement.Elements().Where(e => e.Name.LocalName == nameof(DanceFigureTransition).ToLowerInvariant()))
        {
            var transitionOrError = DanceFigureTransition.FromXml(element, allVertices);
            if (transitionOrError.IsT0)
            {
                graph.AddEdge(transitionOrError.AsT0);
            }
        }

        return graph;
    }
}