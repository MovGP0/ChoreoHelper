using System.Globalization;
using System.Xml.Linq;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Gateway;

public sealed class XmlDataSaver
{
    public void Save(string fileName, IList<Dance> dances)
    {
        var rootElement = new XElement(XNamespaces.ChoreoHelper + "dance");

        var danceElements = dances.Select(ToDanceElement);

        rootElement.Add(danceElements);

        var danceFiguresElements =
            from dance in dances
            from figure in dance.Figures
            select ToDanceFigureElement(dance, figure);

        rootElement.Add(danceFiguresElements);

        var danceFigureTransitionsElements =
            from dance in dances
            from transition in dance.Transitions
            where transition.Distance.IsT0
            select ToDanceFigureTransitionElement(
                dance,
                transition.Source,
                transition.Target,
                transition.Distance);

        rootElement.Add(danceFigureTransitionsElements);

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            rootElement);

        document.Save(fileName);
    }

    private static XElement ToDanceElement(Dance dance)
    {
        return new XElement(XNamespaces.ChoreoHelper + "dance",
            new XAttribute(XNamespaces.ChoreoHelper + "name", dance.Name),
            new XAttribute(XNamespaces.ChoreoHelper + "category", dance.Category));
    }

    private static XElement ToDanceFigureElement(Dance dance, DanceFigure figure)
    {
        return new XElement(XNamespaces.ChoreoHelper + "dancefigure",
            new XAttribute(XNamespaces.ChoreoHelper + "dance", dance.Name),
            new XAttribute(XNamespaces.ChoreoHelper + "name", figure.Name),
            new XAttribute(XNamespaces.ChoreoHelper + "level", figure.Level.ToString("G").ToLowerInvariant()));
    }

    private static XElement ToDanceFigureTransitionElement(Dance dance, DanceFigure source, DanceFigure target, Entities.Distance distance)
    {
        return new XElement(XNamespaces.ChoreoHelper + "dancefiguretransition",
            new XAttribute(XNamespaces.ChoreoHelper + "dance", dance.Name),
            new XAttribute(XNamespaces.ChoreoHelper + "source", source.Name),
            new XAttribute(XNamespaces.ChoreoHelper + "target", target.Name),
            new XAttribute(XNamespaces.ChoreoHelper + "distance", distance.Match(
                distanceValue => distanceValue.ToString("G", CultureInfo.InvariantCulture),
                none => "none",
                unknown => "unknown")));
    }
}