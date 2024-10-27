using System.Xml.Linq;
using ChoreoHelper.Editor.Model;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Editor.Business;

public sealed class XmlDataLoader
{
    public List<Dance> LoadDances(string filePath)
    {
        var ns = XNamespaces.ChoreoHelper;
        var doc = XDocument.Load(filePath);
        if (doc.Root is null)
        {
            return new List<Dance>(0);
        }

        var dances = doc.Root.Elements(ns + "dance")
            .Where(x => x.Attribute(ns + "name") != null)
            .Select(Dance.FromXml)
            .Where(d => d.IsT0)
            .Select(d => d.AsT0)
            .ToList();

        foreach (var x in doc.Root.Elements(ns + "dancefigure"))
        {
            var danceName = x.Attribute(ns + "dance")?.Value;
            if (danceName is null)
            {
                continue;
            }
            
            var dance = dances.Find(d => d.Name == danceName);
            if (dance is null)
            {
                continue;
            }

            var figureOption = DanceFigure.FromXml(x, dances);
            if (figureOption.TryPickT0(out var figure, out _))
            {
                dance.Figures.Add(figure);
            }
        }

        foreach(var t in doc.Root.Elements(ns + "dancefiguretransition"))
        {
            var danceName = t.Attribute(ns + "dance")?.Value;
            if (danceName is null)
            {
                continue;
            }

            var dance = dances.Find(d => d.Name == danceName);
            if (dance is null)
            {
                continue;
            }

            var transitionOption = DanceFigureTransition.FromXml(t, dance.Figures.AsReadOnly());
            if (transitionOption.TryPickT0(out var transition, out _))
            {
                dance.Transitions.Add(transition);
            }
        }

        return dances;
    }
}