namespace ChoreoHelper.Messages;

/// <summary>
/// When a required figure has changed, this message is sent.
/// </summary>
/// <param name="Name">The name of the figure</param>
/// <param name="IsSelected">
/// <c>true</c> when the figure is selected.
/// <c>false</c> when the figure is not selected.</param>
public record RequiredFigureUpdated(string Name, bool IsSelected);