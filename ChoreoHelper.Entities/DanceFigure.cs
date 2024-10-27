﻿namespace ChoreoHelper.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class DanceFigure(Dance dance, string name, DanceLevel level)
{
    [Pure]
    public Dance Dance { get; } = dance;

    [Pure]
    public string Name { get; } = name;

    [Pure]
    public DanceLevel Level { get; } = level;

    [Pure]
    private string DebuggerDisplay => $"{Dance} {Name} {Level}";
}