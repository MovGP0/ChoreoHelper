using ChoreoHelper.Entities;

namespace ChoreoHelper.TransitionEditor.Events;

/// <summary>
/// Shows the editor for a given dance figure.
/// </summary>
/// <param name="Figure">
/// The <see cref="DanceFigure"/> to edit.
/// </param>
public record ShowFigureEditorCommand(DanceFigure Figure);