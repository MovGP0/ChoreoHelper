namespace ChoreoHelper.Graph.Entities;

public sealed partial class Dance
{
    [Pure]
    public XElement ToXml(XNamespace ns) => new(ns + nameof(Dance).ToLowerInvariant(),
        GetAttributes(ns));

    [Pure]
    private IEnumerable<XAttribute> GetAttributes(XNamespace ns)
    {
        yield return new XAttribute(Xn(ns, nameof(Category)), Category);
        yield return new XAttribute(Xn(ns, nameof(Name)), Name);
    }

    [Pure]
    public static OneOf<Dance, Error> FromXml(XElement element)
    {
        if (element.Name.LocalName != nameof(Dance).ToLowerInvariant())
        {
            return new Error();
        }

        var ns = element.Name.Namespace;
        var name = element.Attribute(Xn(ns, nameof(Name)))?.Value ?? string.Empty;
        var category = element.Attribute(Xn(ns, nameof(Category)))?.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
        {
            return new Error();
        }

        return new Dance(category, name);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}