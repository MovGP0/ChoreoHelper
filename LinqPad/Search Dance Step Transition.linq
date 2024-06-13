<Query Kind="Program">
  <NuGetReference>MathNet.Numerics</NuGetReference>
  <NuGetReference>Roslynator.Analyzers</NuGetReference>
  <NuGetReference>SliccDB</NuGetReference>
  <Namespace>MathNet.Numerics.LinearAlgebra.Single</Namespace>
  <Namespace>SliccDB</Namespace>
  <Namespace>SliccDB.Core</Namespace>
  <Namespace>SliccDB.Fluent</Namespace>
  <Namespace>SliccDB.Serialization</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>MathNet.Numerics.LinearAlgebra</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

#load "LogHelper.linq"
#load "BreadthFirstSearch.linq"
#nullable enable

void Main()
{
	var level = DanceLevel.Gold;
	var startFigure = "Underarm Turning Left";
	var endFigure = "Underarm Turning Right";
	byte maxDepth = 3;

	var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
	var baseFolder = Path.GetDirectoryName(Util.CurrentQueryPath);
	var databasePath = Path.Combine(baseFolder, @"dance transitions.sliccdb");
	if (!File.Exists(databasePath))
	{
		LogError($"File '{databasePath}' was not found.");
		return;
	}

	var connection = new DatabaseConnection(databasePath);

	var dances = connection.GetDances().Order().ToArray();
	var dance = AskUserChoice(dances, "Select Dance");
	if (string.IsNullOrEmpty(dance))
	{
		return;
	}
	LogVerbose($"'{dance}' was selected");

	var figures = connection.GetFigures(dance, level).OrderBy(e => e.Name).ToArray();

	startFigure = AskUserChoice(figures.Select(e => e.Name).Order().ToArray(), "Start figure");
	if (string.IsNullOrEmpty(startFigure))
	{
		return;
	}

	endFigure = AskUserChoice(figures.Select(e => e.Name).Order().ToArray(), "End figure");
	if (string.IsNullOrEmpty(endFigure))
	{
		return;
	}

	var matrix = GetDistanceMatrix(connection, figures);
	
	var islands = FindUnreachableIslands(matrix);
	if (islands.Count > 1)
	{
		LogWarning($"There are {islands.Count} unreachable islands.");
	}
	else
	{
		LogInfo("No unreachable steps detected");
	}

	var startIndex = figures.IndexOf(f => f.Name.ToLowerInvariant() == startFigure.ToLowerInvariant());
	var stopIndex = figures.IndexOf(f => f.Name.ToLowerInvariant() == endFigure.ToLowerInvariant());

	DumpOrnament();
	"FORWARD PASS".Dump();
	DumpOrnament();

	var results = BFS(matrix, startIndex, stopIndex, maxDepth, cts.Token);
	foreach(var result in results)
	{
		if(result == null) continue;
		DumpResult(result, figures);
	}

	DumpOrnament();
	"BACKWARD PASS".Dump();
	DumpOrnament();
	results = BFS(matrix, stopIndex, startIndex, maxDepth, cts.Token);
	foreach (var result in results)
	{
		if (result == null) continue;
		DumpResult(result, figures);
	}
}

public static string AskUserChoice(string[] options, string label)
{
	Form form = new()
	{
		Width = 300,
		Height = 150,
		FormBorderStyle = FormBorderStyle.FixedDialog,
		Text = label,
		StartPosition = FormStartPosition.CenterScreen
	};

	ComboBox comboBox = new()
	{
		Left = 50,
		Top = 20,
		Width = 200
	};
	
	foreach (var option in options)
	{
		comboBox.Items.Add(option);
	}

	Button buttonSubmit = new()
	{
		Text = "OK",
		Left = 50,
		Width = 200,
		Top = 50
	};

	buttonSubmit.Click += (sender, e) => { form.Close(); };

	form.Controls.Add(comboBox);
	form.Controls.Add(buttonSubmit);

	form.ShowDialog();
	return comboBox.SelectedItem?.ToString() ?? options[0];
}

static List<List<int>> FindUnreachableIslands(int[,] matrix)
{
	int n = (int)Math.Sqrt(matrix.Length);
	bool[] visited = new bool[n];
	List<List<int>> islands = new List<List<int>>();

	for (int i = 0; i < n; i++)
	{
		if (!visited[i])
		{
			List<int> island = new List<int>();
			DFS(matrix, i, visited, island);
			islands.Add(island);
		}
	}

	return islands;
}

static void DFS(int[,] matrix, int node, bool[] visited, List<int> island)
{
	visited[node] = true;
	island.Add(node);

	for (int i = 0; i < visited.Length; i++)
	{
		if (matrix[node, i] != 0 && !visited[i]) // There is a path and it's not visited
		{
			DFS(matrix, i, visited, island);
		}
	}
}

private static void DumpOrnament()
{
	const int length = 64;
	var ornament = new StringBuilder(64);
	for(var i = 0; i < length; i++)
	{
		ornament.Append("*");
	}
	ornament.ToString().Dump();
}

public static void DumpResult(int[] result, DanceStepNodeInfo[] figures)
{
	var sb = new StringBuilder();
	for (var i = 0; i < result.Length; i++)
	{
		var index = result[i];
		sb.Append(figures[index].Name);

		if (i < result.Length - 1)
		{
			sb.Append(" -> ");
		}
	}
	sb.ToString().Dump();
}

public static class ArrayExtensions
{
	public static int IndexOf<T>(this T[] array, Func<T, bool> predicate)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (predicate(array[i]))
			{
				return i;
			}
		}
		return -1;
	}
}

public static int[,] GetDistanceMatrix(DatabaseConnection connection, DanceStepNodeInfo[] figures)
{
	var matrix = new int[figures.Length, figures.Length];
	for (var row = 0; row < figures.Length; row++)
	{
		for (var col = 0; col < figures.Length; col++)
		{
			var fromFigure = figures[col];
			var toFigure = figures[row];
			int distance = connection.GetDistance(fromFigure, toFigure);
			matrix[row, col] = distance;
		}
	}
	return matrix;
}

public static class DatabaseConnectionExtensions
{
	public static int GetDistance(
		this DatabaseConnection connection,
		DanceStepNodeInfo sourceFigure,
		DanceStepNodeInfo targetFigure)
	{
		var relation = connection.Relations()
			.Where(r => r.SourceHash == sourceFigure.Hash)
			.Where(r => r.TargetHash == targetFigure.Hash)
			.Where(r => r.Properties.Any(p => p.Key == "distance"))
			.FirstOrDefault();

		if (relation == default)
		{
			return -1;
		}

		var value = relation.Properties.First(p => p.Key == "distance").Value;

		if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int valueInt))
		{
			return valueInt;
		}

		return -1;
	}

	public static IEnumerable<string> GetDances(this DatabaseConnection connection)
	{
		return connection
			.Nodes()
			.SelectMany(n => n.Properties)
			.Where(p => p.Key == "dance")
			.Select(p => p.Value)
			.Distinct();
	}

	public static IEnumerable<DanceStepNodeInfo> GetFigures(
		this DatabaseConnection connection,
		string dance,
		DanceLevel level = DanceLevel.All)
	{
		return connection
			.Nodes()
			.WhereDanceName(dance)
			.WhereLevel(level)
			.Select(n => new DanceStepNodeInfo(n.Labels.First(), n.Hash));
	}

	private static IEnumerable<Node> WhereDanceName(this IEnumerable<Node> nodes, string danceName)
	{
		return nodes.Where(e => e.Properties.Any(e => e.Key == "dance" && e.Value.ToLowerInvariant() == danceName.ToLowerInvariant()));
	}

	private static IEnumerable<Node> WhereLevel(this IEnumerable<Node> nodes, DanceLevel level)
	{
		return nodes.Where(e => e.Properties.Any(e => e.Key == "level"
				&& ((e.Value == "bronze" && level.IsFlagSet(DanceLevel.Bronze))
				|| (e.Value == "silver" && level.IsFlagSet(DanceLevel.Silver))
				|| (e.Value == "gold" && level.IsFlagSet(DanceLevel.Gold))
				|| (e.Value == "advanced" && level.IsFlagSet(DanceLevel.Advanced)))));
	}
}

[Flags]
public enum DanceLevel
{
	Bronze = 0b_0001,
	Silver = 0b_0010,
	Gold = 0b_0100,
	Advanced = 0b_1000,
	BronzeAndSilver = Bronze | Silver,
	All = Bronze | Silver | Gold | Advanced
}

public static class DanceLevelExtensions
{
	public static bool IsFlagSet(this DanceLevel level, DanceLevel flagToCheck)
		=> (level & flagToCheck) == flagToCheck;
}

public readonly struct DanceStepNodeInfo
{
	public DanceStepNodeInfo(string name, string hash)
	{
		Name = name;
		Hash = hash;
	}

	public string Name { get; }
	public string Hash { get; }
}