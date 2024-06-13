namespace ChoreoHelper.Behaviors.Algorithms;

public static class DepthFirstSearchUnreachableIslandsFinder
{
    public static List<List<int>> FindUnreachableIslands(int[,] matrix)
    {
        var n = (int)Math.Sqrt(matrix.Length);
        var visited = new bool[n];
        var islands = new List<List<int>>();

        for (var i = 0; i < n; i++)
        {
            if (visited[i]) continue;
            var island = new List<int>();
            DepthFirstSearchRecursive(matrix, i, visited, island);
            islands.Add(island);
        }

        return islands;
    }

    private static void DepthFirstSearchRecursive(
        int[,] matrix,
        int node,
        IList<bool> visited,
        ICollection<int> island)
    {
        visited[node] = true;
        island.Add(node);

        for (var i = 0; i < visited.Count; i++)
        {
            if (matrix[node, i] != 0 && !visited[i])
            {
                // There is a path and it's not visited
                DepthFirstSearchRecursive(matrix, i, visited, island);
            }
        }
    }
}