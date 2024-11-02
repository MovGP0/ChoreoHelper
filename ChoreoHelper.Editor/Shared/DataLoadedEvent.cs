using ChoreoHelper.Entities;

namespace ChoreoHelper.Editor.Shared;

public record DataLoadedEvent(ICollection<Dance> Dances);