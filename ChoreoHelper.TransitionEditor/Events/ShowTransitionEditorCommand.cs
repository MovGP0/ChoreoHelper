using ChoreoHelper.Entities;

namespace ChoreoHelper.TransitionEditor.Events;

/// <summary>
/// Command to show the transition editor.
/// </summary>
/// <param name="Transition">
/// The transition to edit.
/// </param>
public record ShowTransitionEditorCommand(DanceFigureTransition Transition);