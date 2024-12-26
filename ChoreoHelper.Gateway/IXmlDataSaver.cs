using ChoreoHelper.Entities;

namespace ChoreoHelper.Gateway;

public interface IXmlDataSaver
{
    Task SaveAsync(string fileName, IReadOnlyList<Dance> dances, CancellationToken cancellationToken);
}