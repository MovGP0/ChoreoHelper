using ChoreoHelper.Entities;

namespace ChoreoHelper.Messages;

public record DataLoadedEvent(ICollection<Dance> Dances);