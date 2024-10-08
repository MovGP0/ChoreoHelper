﻿namespace ChoreoHelper.Messages;

/// <summary>
/// When a required figure has changed, this message is sent.
/// </summary>
/// <param name="Hash">The hash of the figure</param>
/// <param name="IsSelected">
/// <c>true</c> when the figure is selected.
/// <c>false</c> when the figure is not selected.</param>
public record RequiredFigureUpdated(string Hash, bool IsSelected);