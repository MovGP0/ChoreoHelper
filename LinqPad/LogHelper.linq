<Query Kind="Program" />

// #load "LogHelper.linq"
public void LogInfo(string message) => WriteLineColored(message, "#2196F3"); // Blue
public void LogWarning(string message) => WriteLineColored(message, "#FF9800"); // Orange
public void LogError(string message) => WriteLineColored(message, "#F44336"); // Red
public void LogVerbose(string message) => WriteLineColored(message, "#9E9E9E"); // Grey

private void WriteLineColored(string message, string color)
{
	if (!IsValidHex(color))
		throw new ArgumentException("Invalid color. Color should be a valid hex code.");

	Util.WithStyle(message, $"color:{color}").Dump();
}

private static readonly Regex HexNumberRegex = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.NonBacktracking);
private bool IsValidHex(string hex) => HexNumberRegex.IsMatch(hex);