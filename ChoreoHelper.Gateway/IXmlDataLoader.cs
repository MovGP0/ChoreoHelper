using ChoreoHelper.Entities;

namespace ChoreoHelper.Gateway;

public interface IXmlDataLoader
{
    Task<List<Dance>> LoadDancesAsync(string filePath, CancellationToken cancellationToken);
}