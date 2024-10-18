using System.Xml.Linq;
using ChoreHelper.Editor.Model;

namespace ChoreHelper.Editor.Business;

public sealed class XmlDataSaver
{
    public void Save(string fileName, IList<Dance> dances)
    {
        var rootElement = new XElement(XNamespace.None + "dance");

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
        return new XElement(XNamespace.None + "dance",
            new XAttribute(XNamespace.None + "name", dance.Name),
            new XAttribute(XNamespace.None + "category", dance.Category));
    }

    private static XElement ToDanceFigureElement(Dance dance, DanceFigure figure)
    {
        return new XElement(XNamespace.None + "dancefigure",
            new XAttribute(XNamespace.None + "dance", dance.Name),
            new XAttribute(XNamespace.None + "name", figure.Name),
            new XAttribute(XNamespace.None + "level", figure.Level.ToString("G").ToLowerInvariant()));
    }

    private static XElement ToDanceFigureTransitionElement(Dance dance, DanceFigure source, DanceFigure target, byte distance)
    {
        return new XElement(XNamespace.None + "dancefiguretransition",
            new XAttribute(XNamespace.None + "dance", dance.Name),
            new XAttribute(XNamespace.None + "source", source.Name),
            new XAttribute(XNamespace.None + "target", target.Name),
            new XAttribute(XNamespace.None + "distance", distance));
    }
}