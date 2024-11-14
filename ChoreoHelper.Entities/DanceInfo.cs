namespace ChoreoHelper.Entities;

[DebuggerDisplay("{Name} ({Category})")]
public readonly record struct DanceInfo(string Category, string Name, string Hash);