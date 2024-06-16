<Query Kind="Program">
  <NuGetReference>EPPlus</NuGetReference>
  <NuGetReference>FluentValidation</NuGetReference>
  <NuGetReference>Microsoft.Data.SqlClient</NuGetReference>
  <NuGetReference>Roslynator.Analyzers</NuGetReference>
  <NuGetReference>SliccDB</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
  <Namespace>OfficeOpenXml</Namespace>
  <Namespace>SliccDB</Namespace>
  <Namespace>SliccDB.Serialization</Namespace>
  <Namespace>SliccDB.Fluent</Namespace>
  <Namespace>OfficeOpenXml.Style</Namespace>
  <Namespace>OfficeOpenXml.Drawing</Namespace>
  <Namespace>SliccDB.Core</Namespace>
  <Namespace>System.Globalization</Namespace>
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
		EnsureThatAllDanceStepsExist(worksheet, connection);
		EnsureThatAllTransitionsExist(worksheet, connection);
	}
}

void EnsureThatAllTransitionsExist(ExcelWorksheet worksheet, DatabaseConnection connection)
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

private void EnsureThatAllDanceStepsExist(ExcelWorksheet worksheet, DatabaseConnection connection)
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
	}
	connection.SaveDatabase();
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