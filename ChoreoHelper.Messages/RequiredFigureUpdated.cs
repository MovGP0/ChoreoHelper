namespace ChoreoHelper.Messages;

/// <summary>
/// When a required figure has changed, this message is sent.
/// </summary>
/// <param name="Hash">The hash of the figure</param>
public record RequiredFigureUpdated(string Hash);