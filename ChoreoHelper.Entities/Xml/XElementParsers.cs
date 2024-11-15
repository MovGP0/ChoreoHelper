using System.Globalization;

namespace ChoreoHelper.Entities.Xml;

public static class XElementParsers
{
    public static CompetitionRestriction ParseRestriction(XElement element)
    {
        var ns = element.Name.Namespace;
        var restrictionValue = element.Attribute(Xn(ns, "Restriction"))?.Value ?? string.Empty;
        var restriction = CompetitionRestriction.AllowedInAllClasses;
        if (int.TryParse(restrictionValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int restrictionInt))
        {
            restriction = (CompetitionRestriction)restrictionInt;
        }

        return restriction;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}