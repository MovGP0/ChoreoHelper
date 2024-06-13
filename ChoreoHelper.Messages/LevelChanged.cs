using ChoreoHelper.Database;

namespace ChoreoHelper.Messages;

public record LevelChanged(DanceLevel Level, bool IsSelected);