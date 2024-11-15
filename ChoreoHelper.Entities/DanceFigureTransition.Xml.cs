using System.Globalization;
using static ChoreoHelper.Entities.Xml.XElementParsers;

namespace ChoreoHelper.Entities;

public sealed partial class DanceFigureTransition
{
    [Pure]
    public XElement ToXml(XNamespace ns) => new(ns + nameof(DanceFigureTransition).ToLowerInvariant(), GetAttributes(ns));

    [Pure]
    private IEnumerable<XAttribute> GetAttributes(XNamespace ns)
    {
        yield return new XAttribute(Xn(ns, nameof(Source)), Source.Name);
        yield return new XAttribute(Xn(ns, nameof(Target)), Target.Name);
        yield return new XAttribute(Xn(ns, nameof(Distance)), Distance.TryPickT0(out var distance, out _) ? distance.ToString(CultureInfo.InvariantCulture) : "");
        yield return new XAttribute(Xn(ns, nameof(Restriction)), Restriction.ToString("D"));
    }

    [Pure]
    public static OneOf<DanceFigureTransition, Error> FromXml(XElement element, IReadOnlyCollection<DanceFigure> figures)
    {
        if (element.Name.LocalName != nameof(DanceFigureTransition).ToLowerInvariant())
        {
            return new Error();
        }

        var danceName = GetDanceName(element);
        if (string.IsNullOrWhiteSpace(danceName))
        {
            return new Error();
        }

        var sourceFigure = TryFindSourceFigure(element, figures, danceName);
        if (!sourceFigure.TryPickT0(out var source, out _))
        {
            return new Error();
        }

        var targetFigure = TryFindTargetFigure(element, figures, danceName);
        if (!targetFigure.TryPickT0(out var target, out _))
        {
            return new Error();
        }

        var distance = TryParseDistance(element);

        var restriction = ParseRestriction(element);

        return new DanceFigureTransition(source, target, distance, restriction);
    }

    private static string GetDanceName(XElement element)
    {
        var ns = element.Name.Namespace;
        return element.Attribute(Xn(ns, "Dance"))?.Value ?? string.Empty;
    }

    private static Distance TryParseDistance(XElement element)
    {
        var ns = element.Name.Namespace;
        var distanceValue = element.Attribute(Xn(ns, nameof(Distance)))?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(distanceValue))
        {
            return new None();
        }

        if (float.TryParse(distanceValue, CultureInfo.InvariantCulture, out var distance))
        {
            return distance;
        }

        return new None();
    }

    private static OneOf<DanceFigure, Error> TryFindTargetFigure(
        XElement element,
        IReadOnlyCollection<DanceFigure> figures,
        string danceName)
    {
        var ns = element.Name.Namespace;
        var sourceName = element.Attribute(Xn(ns, nameof(Target)))?.Value ?? string.Empty;
        var source = figures
            .Where(f => f.Name == sourceName && f.Dance.Name == danceName)
            .Take(1)
            .ToArray();

        if (source.Length != 1)
        {
            return new Error();
        }

        return source[0];
    }
    
    private static OneOf<DanceFigure, Error> TryFindSourceFigure(
        XElement element,
        IReadOnlyCollection<DanceFigure> figures,
        string danceName)
    {
        var ns = element.Name.Namespace;
        var sourceName = element.Attribute(Xn(ns, nameof(Source)))?.Value ?? string.Empty;
        var source = figures
            .Where(f => f.Name == sourceName && f.Dance.Name == danceName)
            .Take(1)
            .ToArray();

        if (source.Length != 1)
        {
            return new Error();
        }

        return source[0];
    }
}