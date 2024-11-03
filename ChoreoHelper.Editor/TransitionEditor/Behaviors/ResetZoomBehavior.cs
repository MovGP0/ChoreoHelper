using System.Reactive.Disposables;
using ChoreoHelper.Editor.TransitionEditor.Events;
using MessagePipe;
using ReactiveUI;
using SkiaSharp;

namespace ChoreoHelper.Editor.TransitionEditor.Behaviors;

public sealed class ResetZoomBehavior(
    IPublisher<RenderTransitionEditorCommand> renderTransitionEditorPublisher)
    : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        var command = ReactiveCommand.Create(() => { }).DisposeWith(disposables);

        command.Subscribe(_ =>
        {
            viewModel.TransformationMatrix = SKMatrix.CreateIdentity();
            renderTransitionEditorPublisher.Publish(new RenderTransitionEditorCommand());
        });

        viewModel.ResetZoom = command;
    }
}