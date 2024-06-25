namespace ChoreoHelper.Graph.Entities;

public sealed partial class DanceFigure
{
    [Pure]
    public XElement ToXml(XNamespace ns) => new(ns + nameof(DanceFigure).ToLowerInvariant(), GetAttributes(ns));

    [Pure]
    private IEnumerable<XAttribute> GetAttributes(XNamespace ns)
    {
        yield return new XAttribute(Xn(ns, nameof(Dance)), Dance);
        yield return new XAttribute(Xn(ns, nameof(Name)), Name);
        yield return new XAttribute(Xn(ns, nameof(Level)), Level);
    }

    [Pure]
    public static OneOf<DanceFigure, Error> FromXml(XElement element)
    {
        if (element.Name.LocalName != nameof(DanceFigure).ToLowerInvariant())
        {
            return new Error();
        }

        var ns = element.Name.Namespace;
        var dance = element.Attribute(Xn(ns, nameof(Dance)))?.Value ?? string.Empty;
        var name = element.Attribute(Xn(ns, nameof(Name)))?.Value ?? string.Empty;
        var level = element.Attribute(Xn(ns, nameof(Level)))?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(dance) || string.IsNullOrWhiteSpace(level))
        {
            return new Error();
        }

        return new DanceFigure(dance, name, level);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}