using ChoreoHelper.Entities;

namespace ChoreoHelper.Algorithms;

public interface IUnreachableIslandsFinder
{
    List<List<int>> FindUnreachableIslands(Distance[,] matrix);
}