<Query Kind="Statements">
  <NuGetReference>Shouldly</NuGetReference>
  <Namespace>Shouldly</Namespace>
  <Namespace>Xunit</Namespace>
</Query>

// Press Alt+Shift+T to run all tests and check the result.
#load "xunit"
#load "BreadthFirstSearch.linq"
#nullable enable

[Fact]
void Test_BFS_2x2Matrix()
{
	using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
	int[,] matrix =
	{
		{ -1, 1 },
		{ 1, -1 }
	};

	var result = BFS(matrix, 0, 1, 5, cts.Token).ToArray();

	// There should be exactly one path
	result.Length.ShouldBe(1);

	// That path should be 0 -> 1
	result[0].ShouldBe(new[] { 0, 1 });
}

[Fact]
void Test_BFS_3x3Matrix()
{
	using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
	int[,] matrix =
	{
		{ -1, 1, -1 },
		{ -1, -1, 1 },
		{ 1, -1, -1 }
	};

	var result = BFS(matrix, 0, 2, maxDepth: 3, cts.Token).ToArray();

	// There should be exactly one path
	result.Length.ShouldBe(1);

	// That path should be 0 -> 1
	result[0].ShouldBe(new[] { 0, 1, 2 });
}