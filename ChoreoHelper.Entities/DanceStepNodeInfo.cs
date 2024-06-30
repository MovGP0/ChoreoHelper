namespace ChoreoHelper.Entities;

public readonly struct DanceStepNodeInfo
{
    public DanceStepNodeInfo(string name, string hash, DanceLevel level)
    {
        Name = name;
        Hash = hash;
        Level = level;
    }

    public string Name { get; }
    public string Hash { get; }
    public DanceLevel Level { get; }
}