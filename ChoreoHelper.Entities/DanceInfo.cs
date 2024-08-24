namespace ChoreoHelper.Entities;

public readonly record struct DanceInfo
{
    public DanceInfo(string category, string name, string hash)
    {
        Category = category;
        Name = name;
        Hash = hash;
    }

    public string Category { get; }
    public string Name { get; }
    public string Hash { get; }
}