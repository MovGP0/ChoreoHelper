using System.Xml.Linq;
using ChoreHelper.Editor.Model;
using ChoreoHelper.Entities;

namespace ChoreHelper.Editor.Business;

public sealed class XmlDataLoader
{
    public List<Dance> LoadDances(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);
        if (doc.Root is null)
        {
            return new List<Dance>(0);
        }

        var dances = doc.Root.Elements("dance")
            .Where(x => x.Attribute("name") != null)
            .Select(x => new Dance
            {
                Name = x.Attribute("name").Value,
                Category = x.Attribute("category").Value
            }).ToList();

        foreach (var x in doc.Root.Elements("dancefigure"))
        {
            var danceName = x.Attribute("dance").Value;
            var dance = dances.Single(d => d.Name == danceName);

            var level = x.Attribute("level").Value switch
            {
                "bronze" => DanceLevel.Bronze,
                "silver" => DanceLevel.Silver,
                "gold" => DanceLevel.Gold,
                "advanced" => DanceLevel.Advanced,
                _ => DanceLevel.Undefined
            };

            var figure = new DanceFigure
            {
                Name = x.Attribute("name").Value,
                Level = level
            };

            dance.Figures.Add(figure);
        }

        foreach(var t in doc.Root.Elements("dancefiguretransition"))
        {
            var danceName = t.Attribute("dance").Value;
            Dance dance = dances.Single(d => d.Name == danceName);

            var sourceName = t.Attribute("source").Value;
            var sourceFigure = dance.Figures.Single(f => f.Name == sourceName);

            var targetName = t.Attribute("target").Value;
            var targetFigure = dance.Figures.Single(f => f.Name == targetName);

            var distance = byte.Parse(t.Attribute("distance").Value);

            dance.Transitions.Add(new Transition(sourceFigure, targetFigure, distance));
        }

        return dances;
    }
}