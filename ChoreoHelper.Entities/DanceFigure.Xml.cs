namespace ChoreoHelper.Entities;

public sealed partial class DanceFigure
{
    [Pure]
    public XElement ToXml(XNamespace ns) => new(ns + nameof(DanceFigure).ToLowerInvariant(),
        GetAttributes(ns));

    [Pure]
    private IEnumerable<XAttribute> GetAttributes(XNamespace ns)
    {
        yield return new XAttribute(Xn(ns, nameof(Dance)), Dance.Name);
        yield return new XAttribute(Xn(ns, nameof(Name)), Name);
        yield return new XAttribute(Xn(ns, nameof(Level)), Level);
    }

    [Pure]
    public static OneOf<DanceFigure, Error> FromXml(XElement element, IEnumerable<Dance> dances)
    {
        if (element.Name.LocalName != nameof(DanceFigure).ToLowerInvariant())
        {
            return new Error();
        }

        var ns = element.Name.Namespace;
        var danceName = element.Attribute(Xn(ns, nameof(Dance)))?.Value ?? string.Empty;
        var name = element.Attribute(Xn(ns, nameof(Name)))?.Value ?? string.Empty;
        var level = element.Attribute(Xn(ns, nameof(Level)))?.Value ?? string.Empty;

        var danceLevel = level switch
        {
            "bronze" => DanceLevel.Bronze,
            "silver" => DanceLevel.Silver,
            "gold" => DanceLevel.Gold,
            "advanced" => DanceLevel.Advanced,
            _ => DanceLevel.Undefined
        };
        
        if (string.IsNullOrWhiteSpace(danceName)
            || string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(level))
        {
            return new Error();
        }

        var dance = dances
            .Where(d => d.Name == danceName)
            .Take(1)
            .ToArray();

        if (dance.Length != 1)
        {
            return new Error();
        }

        return new DanceFigure(dance[0], name, danceLevel);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}