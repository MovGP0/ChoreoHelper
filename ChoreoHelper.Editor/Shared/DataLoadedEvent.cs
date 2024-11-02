using ChoreoHelper.Entities;

namespace ChoreoHelper.Editor.Messages;

public record DataLoadedEvent(ICollection<Dance> Dances);