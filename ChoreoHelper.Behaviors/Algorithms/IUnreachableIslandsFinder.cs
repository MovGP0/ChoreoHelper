namespace ChoreoHelper.Behaviors.Algorithms;

public interface IUnreachableIslandsFinder
{
    List<List<int>> FindUnreachableIslands(int[,] matrix);
}