﻿namespace ChoreoHelper.Graph.Entities;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class DanceFigure(Dance dance, string name, string level)
{
    [Pure]
    public Dance Dance { get; } = dance;

    [Pure]
    public string Name { get; } = name;

    [Pure]
    public string Level { get; } = level;

    [Pure]
    private string DebuggerDisplay => $"{Dance} {Name} {Level}";
}