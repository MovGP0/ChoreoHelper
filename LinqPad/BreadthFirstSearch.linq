<Query Kind="Program" />

#nullable enable

public IEnumerable<int[]> BFS(
	int[,] matrix,
	int startNode,
	int targetNode,
	byte maxDepth = 5,
	CancellationToken token = default)
{
	var n = matrix.GetLength(0);
	var queue = new Queue<NodePath>();
	queue.Enqueue(new NodePath(new[] { startNode }, 0));

	while (queue.Any())
	{
		if (token.IsCancellationRequested)
		{
			throw new OperationCanceledException(token);
		}

		var current = queue.Dequeue();
		var currentPath = current.Path;
		var currentDistance = current.Distance;
		var lastNode = currentPath.Last();

		if (lastNode == targetNode)
		{
			yield return currentPath;
		}

		if (currentDistance < maxDepth)
		{
			for (var i = 0; i < n; i++)
			{
				var distance = matrix[lastNode, i];
				if (distance >= 0 && !currentPath.Contains(i))
				{
					var newPath = new List<int>(currentPath) { i }.ToArray();
					queue.Enqueue(new NodePath(newPath, currentDistance + distance));
				}
			}
		}
	}
}

public readonly struct NodePath
{
	public NodePath(int[] path, int distance)
	{
		Path = path;
		Distance = distance;
	}

	public int[] Path { get; } = { };
	public int Distance { get; }
}