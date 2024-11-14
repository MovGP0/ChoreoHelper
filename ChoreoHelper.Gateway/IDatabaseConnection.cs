﻿using System.Collections.Immutable;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Gateway;

public interface IDanceFiguresRepository
{
    (OneOf<float, None>[,] array, DanceStepNodeInfo[] figures) GetDistanceMatrix(string dance, DanceStepNodeInfo[] figures);
    IEnumerable<DanceInfo> GetDances();
    IEnumerable<DanceStepNodeInfo> GetFigures(string? dance, DanceLevel level = DanceLevel.All);
    IImmutableSet<DanceLevel> GetDanceLevels();
}