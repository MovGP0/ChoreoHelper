using System.Globalization;

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
        yield return new XAttribute(Xn(ns, nameof(Distance)), Distance.ToString(CultureInfo.InvariantCulture));
    }

    [Pure]
    public static OneOf<DanceFigureTransition, Error> FromXml(XElement element, IReadOnlyCollection<DanceFigure> figures)
    {
        if (element.Name.LocalName != nameof(DanceFigureTransition).ToLowerInvariant())
        {
            return new Error();
        }

        var ns = element.Name.Namespace;

        var danceName = element.Attribute(Xn(ns, "Dance"))?.Value ?? string.Empty;

        var sourceName = element.Attribute(Xn(ns, nameof(Source)))?.Value ?? string.Empty;
        var source = figures
            .Where(f => f.Name == sourceName && f.Dance.Name == danceName)
            .Take(1)
            .ToArray();

        if (source.Length != 1)
        {
            return new Error();
        }

        var targetName = element.Attribute(Xn(ns, nameof(Target)))?.Value ?? string.Empty;
        var target = figures
            .Where(f => f.Name == targetName && f.Dance.Name == danceName)
            .Take(1)
            .ToArray();

        if (target.Length != 1)
        {
            return new Error();
        }

        var distanceValue = element.Attribute(Xn(ns, nameof(Distance)))?.Value ?? string.Empty;
        if (float.TryParse(distanceValue, CultureInfo.InvariantCulture, out float distance))
        {
            return new DanceFigureTransition(source[0], target[0], distance);
        }

        return new Error();
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}