<Query Kind="Program">
  <NuGetReference>EPPlus</NuGetReference>
  <NuGetReference>FluentValidation</NuGetReference>
  <NuGetReference Version="5.2.1">Microsoft.Data.SqlClient</NuGetReference>
  <NuGetReference>Microsoft.Identity.Client</NuGetReference>
  <NuGetReference>OneOf</NuGetReference>
  <NuGetReference>QuikGraph</NuGetReference>
  <NuGetReference>Roslynator.Analyzers</NuGetReference>
  <NuGetReference>SliccDB</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
  <Namespace>OfficeOpenXml</Namespace>
  <Namespace>OfficeOpenXml.Drawing</Namespace>
  <Namespace>OfficeOpenXml.Style</Namespace>
  <Namespace>OneOf</Namespace>
  <Namespace>OneOf.Types</Namespace>
  <Namespace>QuikGraph</Namespace>
  <Namespace>SliccDB</Namespace>
  <Namespace>SliccDB.Core</Namespace>
  <Namespace>SliccDB.Fluent</Namespace>
  <Namespace>SliccDB.Serialization</Namespace>
  <Namespace>System.Diagnostics.Contracts</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
</Query>

#load "LogHelper.linq"
#nullable enable

void Main()
{
	var baseFolder = Path.GetDirectoryName(Util.CurrentQueryPath);
	var excelPath = Path.Combine(baseFolder, "..", @"dance transitions.xlsx");
	if (!File.Exists(excelPath))
	{
		LogError($"File '{excelPath}' was not found.");
		return;
	}

	var databasePath = Path.Combine(baseFolder, "..", @"dance transitions.sliccdb");
	if(!File.Exists(databasePath))
	{
		LogError($"File '{databasePath}' was not found.");
		return;
	}

	using ExcelPackage package = new(new FileInfo(excelPath));
	var graph = new UndirectedGraph<DanceFigure, DanceFigureTransition>();

	foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets.Where(ws => !ws.Name.StartsWith("_")))
	{
		var validationResult = ValidateWorksheet(worksheet);
		if(!validationResult.IsValid)
		{
			LogResult(validationResult);
			return;
		}
		else
		{
			LogVerbose($"Worksheet '{worksheet.Name}' validated successfully.");
		}
		
		var connection = new DatabaseConnection(databasePath);
		EnsureThatAllDanceStepsExist(worksheet, connection, graph);
		EnsureThatAllTransitionsExist(worksheet, connection, graph);
	}

	SaveGraph(graph);
}

public sealed partial class DanceFigure
{
	public DanceFigure(string dance, string name, string level)
	{
		Dance = dance;
		Name = name;
		Level = level;
	}
	
	public string Dance { get; }
	public string Name { get; }
	public string Level { get; }
}

public sealed partial class DanceFigureTransition : IEdge<DanceFigure>
{
	public DanceFigureTransition(DanceFigure source, DanceFigure target, float distance)
	{
		Source = source;
		Target = target;
		Distance = distance;
	}

	public DanceFigure Source { get; }
	public DanceFigure Target { get; }
	public float Distance { get; }
}

void EnsureThatAllTransitionsExist(ExcelWorksheet worksheet, DatabaseConnection connection, UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
{
	var rows = worksheet.Dimension.End.Row;
	var columns = worksheet.Dimension.End.Column;
	
	var danceName = worksheet.Name;

	for (int row = 2; row <= rows; row++)
	{
		for (int col = 2; col <= columns; col++)
		{
			var fromDanceStep = worksheet.Cells[row, 1].Value.ToString()!;
			var toDanceStep = worksheet.Cells[1, col].Value.ToString()!;
			var cell = worksheet.Cells[row, col];
			if (cell?.Value is null)
			{
				LogWarning($"[{danceName}] row {row} col {col} was empty");
				continue;
			}
			var distance = ToDistance(cell.Value.ToString());
			if (distance == null) continue;

			LogVerbose($"{fromDanceStep} ==[{distance}]=> {toDanceStep}");

			var fromNode = connection.GetDanceStepNode(danceName, fromDanceStep);
			Debug.Assert(fromNode != null);

			var toNode = connection.GetDanceStepNode(danceName, toDanceStep);
			Debug.Assert(toNode != null);

			connection.SetDistance(fromNode, toNode, distance.Value);
			//************************************************************************************
			var f = graph.Vertices.Single(v => v.Name == fromDanceStep && v.Dance == danceName);
			var t = graph.Vertices.Single(v => v.Name == toDanceStep && v.Dance == danceName);
			var edge = new DanceFigureTransition(f, t, (float)distance);
			graph.AddEdge(edge);
		}
	}

	connection.SaveDatabase();
}

public int? ToDistance(string? cellvalue)
{
	if (cellvalue == "G") return 1;
	if (cellvalue == "O") return 2;
	return null;
}

public static class DatabaseConnectionExtensions
{
	public static void SetDistance(
		this DatabaseConnection connection,
		Node from,
		Node to,
		int distance)
	{
		var distString = distance.ToString(CultureInfo.InvariantCulture);
		
		var existingRelation = connection
			.Relations()
			.Where(r => r.SourceHash == from.Hash)
			.Where(r => r.TargetHash == to.Hash)
			.FirstOrDefault();

		if (existingRelation == default)
		{
			var properties = new Dictionary<string, string>();
			properties["distance"] = distance.ToString(CultureInfo.InvariantCulture);
			connection.CreateRelation(Guid.NewGuid().ToString(), from, to, properties, null);
			return;
		}
		
		existingRelation.Properties["distance"] = distString;
	}
	
	public static void CreateRelation(
		this DatabaseConnection connection,
		string relationName,
		Node sourceNode,
		Node targetNode,
		Dictionary<string, string>? properties = null,
		HashSet<string>? labels = null)
	{
		connection.CreateRelation(
			Guid.NewGuid().ToString(),
			sn => sn.First(x => x.Hash == sourceNode.Hash),
			sn => sn.First(x => x.Hash == targetNode.Hash),
			properties,
			labels);
	}

	public static Node? GetDanceStepNode(this DatabaseConnection connection, string danceName, string danceStepName)
	{
		return connection
			.Nodes()
			.Properties("dance".Value(danceName))
			.Labels(danceStepName)
			.FirstOrDefault();
	}
}

private void EnsureThatAllDanceStepsExist(ExcelWorksheet worksheet, DatabaseConnection connection, UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
{
	var columns = worksheet.Dimension.End.Column;
	for (int col = 2; col <= columns; col++)
	{
		var danceName = worksheet.Name;
		var danceStepName = worksheet.Cells[1, col].Value.ToString();
		var level = GetColorForExcelColor(worksheet.Cells[1, col].Style.Fill.BackgroundColor);

		var selectedNode = connection
			.Nodes()
			.Properties("dance".Value(danceName))
			.Labels(danceStepName)
			.FirstOrDefault();

		if (selectedNode == default)
		{
			var properties = new Dictionary<string, string>();
			var labels = new HashSet<string>();
			properties.Add("dance", danceName);
			properties.Add("level", level);
			labels.Add(danceStepName);
			_ = connection.CreateNode(properties, labels);
			LogVerbose($"Created node [{danceName}] {danceStepName}");
		}
		else
		{
			selectedNode.Properties["level"] = level;
		}
		
		var vertex = new DanceFigure(danceName, danceStepName!, level);
		graph.AddVertex(vertex);
	}

	connection.SaveDatabase();
}

private void SaveGraph(UndirectedGraph<DanceFigure, DanceFigureTransition> graph)
{
	var baseFolder = Path.GetDirectoryName(Util.CurrentQueryPath);
	var xmlPath = Path.Combine(baseFolder, "..", @"dance transitions.xml");
	var rootElement = new XElement("dance");

	foreach(var v in graph.Vertices)
	{
		var x = v.ToXml(XNamespace.None);
		rootElement.Add(x);
	}

	foreach(var e in graph.Edges)
	{
		var x = e.ToXml(XNamespace.None);
		rootElement.Add(x);
	}

	var document = new XDocument(rootElement);
	document.Save(xmlPath);
}

private string GetColorForExcelColor(ExcelColor color)
{
	if (color.Theme == eThemeSchemeColor.Accent2) return "bronze";
	if (color.Theme == eThemeSchemeColor.Accent3) return "silver";
	if (color.Theme == eThemeSchemeColor.Accent4) return "gold";
	return "";
}

void LogResult(ValidationResult result)
{
	foreach(var res in result.Errors)
	{
		switch(res.Severity)
		{
			case Severity.Error: LogError(res.ErrorMessage); continue;
			case Severity.Info: LogInfo(res.ErrorMessage); continue;
			case Severity.Warning: LogWarning(res.ErrorMessage); continue;
		}
	}
}

private ValidationResult ValidateWorksheet(ExcelWorksheet worksheet)
{
	var result = new ValidationResult();
	var rows = worksheet.Dimension.End.Row;
	var columns = worksheet.Dimension.End.Column;
	
	// Validate number of rows and colums
	if (rows != columns)
	{
		var message = $"Error in Worksheet {worksheet.Name}: Number of columns ({columns}) does not match number of rows ({rows}).";
		result.Errors.Add(new ValidationFailure(worksheet.Name, message) { Severity = Severity.Error });
	}

    // Validate row and column headers
	for (int col = 2; col <= columns; col++)
	{
		if (worksheet.Cells[1, col].Value.ToString() != worksheet.Cells[col, 1].Value.ToString())
		{
			var message = $"Error in Worksheet {worksheet.Name}: Column {ExcelColumnName(col)} does not match row {col}";
			result.Errors.Add(new ValidationFailure(worksheet.Name, message) { Severity = Severity.Error });
		}
	}

    // Validate cell values
	for (int row = 2; row <= rows; row++)
	{
		for (int col = 2; col <= columns; col++)
		{
			string cellValue = worksheet.Cells[row, col].Value?.ToString() ?? "";
			if (!cellValue.Equals("G", StringComparison.InvariantCultureIgnoreCase) &&
				!cellValue.Equals("O", StringComparison.InvariantCultureIgnoreCase) &&
				!cellValue.Equals("X", StringComparison.InvariantCultureIgnoreCase) &&
				!string.IsNullOrWhiteSpace(cellValue))
			{
				var failure = new ValidationFailure($"{worksheet.Name}[{row},{col}]", $"Invalid value '{cellValue}' in cell {ExcelCellBase.GetAddress(row, col)}");
				failure.Severity = FluentValidation.Severity.Warning;
				result.Errors.Add(failure);
			}
		}
	}

	return result;
}

public static string ExcelColumnName(int columnNumber)
{
	if (columnNumber < 0 || columnNumber > ExcelPackage.MaxColumns - 1)
	{
		var message = $"Column number must be between {0} and {ExcelPackage.MaxColumns - 1}";
		throw new ArgumentOutOfRangeException(nameof(columnNumber), columnNumber, message);
	}
	return ExcelCellBase.GetAddressCol(columnNumber + 1);
}

public sealed partial class DanceFigureTransition
{
	[Pure]
	public XElement ToXml(XNamespace ns) => new(ns + nameof(DanceFigureTransition).ToLowerInvariant(), GetAttributes(ns));

	[Pure]
	private IEnumerable<XAttribute> GetAttributes(XNamespace ns)
	{
		yield return new XAttribute(Xn(ns, "Dance"), Source.Dance);
		yield return new XAttribute(Xn(ns, nameof(Source)), Source.Name);
		yield return new XAttribute(Xn(ns, nameof(Target)), Target.Name);
		yield return new XAttribute(Xn(ns, nameof(Distance)), Distance.ToString(CultureInfo.InvariantCulture));
	}

	[Pure]
	public static OneOf<DanceFigureTransition, Error> FromXml(XElement element, IReadOnlyList<DanceFigure> figures)
	{
		if (element.Name.LocalName != nameof(DanceFigureTransition).ToLowerInvariant())
		{
			return new Error();
		}

		var ns = element.Name.Namespace;

		var sourceName = element.Attribute(Xn(ns, nameof(Source)))?.Value ?? string.Empty;
		var source = figures.Where(f => f.Name == sourceName).Take(1).ToArray();
		if (source.Length != 1)
		{
			return new Error();
		}

		var targetName = element.Attribute(Xn(ns, nameof(Target)))?.Value ?? string.Empty;
		var target = figures.Where(f => f.Name == targetName).Take(1).ToArray();
		if (target.Length != 1)
		{
			return new Error();
		}

		var distanceValue = element.Attribute(Xn(ns, nameof(Distance)))?.Value ?? string.Empty;
		if (float.TryParse(distanceValue, CultureInfo.InvariantCulture, out float distance))
		{
			return new DanceFigureTransition(source[0], target[0], distance);
		}

		return new Error();
	}

	[Pure]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}


public sealed partial class DanceFigure
{
	[Pure]
	public XElement ToXml(XNamespace ns) => new(ns + nameof(DanceFigure).ToLowerInvariant(), GetAttributes(ns));

	[Pure]
	private IEnumerable<XAttribute> GetAttributes(XNamespace ns)
	{
		yield return new XAttribute(Xn(ns, nameof(Dance)), Dance);
		yield return new XAttribute(Xn(ns, nameof(Name)), Name);
		yield return new XAttribute(Xn(ns, nameof(Level)), Level);
	}

	[Pure]
	public static OneOf<DanceFigure, Error> FromXml(XElement element)
	{
		if (element.Name.LocalName != nameof(DanceFigure).ToLowerInvariant())
		{
			return new Error();
		}

		var ns = element.Name.Namespace;
		var dance = element.Attribute(Xn(ns, nameof(Dance)))?.Value ?? string.Empty;
		var name = element.Attribute(Xn(ns, nameof(Name)))?.Value ?? string.Empty;
		var level = element.Attribute(Xn(ns, nameof(Level)))?.Value ?? string.Empty;

		if (string.IsNullOrWhiteSpace(dance) || string.IsNullOrWhiteSpace(level))
		{
			return new Error();
		}

		return new DanceFigure(dance, name, level);
	}

	[Pure]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XName Xn(XNamespace ns, string name) => ns + name.ToLowerInvariant();
}