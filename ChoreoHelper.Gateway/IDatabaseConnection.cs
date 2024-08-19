using System.Collections.Immutable;
using ChoreoHelper.Entities;

namespace ChoreoHelper.Gateway;

public interface IDanceFiguresRepository
{
    (int[,] array, DanceStepNodeInfo[] figures) GetDistanceMatrix(string dance, DanceStepNodeInfo[] figures);
    IEnumerable<string> GetDances();
    IEnumerable<DanceStepNodeInfo> GetFigures(string? dance, DanceLevel level = DanceLevel.All);
    IImmutableSet<DanceLevel> GetDanceLevels();
}