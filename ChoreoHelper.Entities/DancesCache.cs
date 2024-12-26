namespace ChoreoHelper.Entities;

/// <summary>
/// Cache of dances
/// </summary>
public sealed partial class DancesCache
{
    private readonly SourceCache<Dance, string> _dances = new(dance => dance.Name);
}