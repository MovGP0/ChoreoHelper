namespace ChoreoHelper.Entities;

[DebuggerDisplay("{Name} ({Level})")]
public readonly record struct DanceStepNodeInfo(string Name, string Hash, DanceLevel Level);