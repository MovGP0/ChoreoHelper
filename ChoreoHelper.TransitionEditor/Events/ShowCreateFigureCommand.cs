using ChoreoHelper.Entities;

namespace ChoreoHelper.TransitionEditor.Events;

/// <summary>
/// Shows the control for creating a dance figure.
/// </summary>
/// <param name="Dance">
/// The <see cref="Dance"/> to which the figure should be added after creation.
/// </param>
public record ShowCreateFigureCommand(Dance Dance);