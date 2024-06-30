using ChoreoHelper.Entities;

namespace ChoreoHelper.Messages;

public record LevelChanged(DanceLevel Level, bool IsSelected);