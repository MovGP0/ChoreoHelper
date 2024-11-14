using System.Reactive.Disposables;
using ChoreoHelper.Editor.TransitionEditor.Events;
using MessagePipe;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Splat;

namespace ChoreoHelper.Editor.TransitionEditor;

public partial class TransitionEditorControl
{
    public TransitionEditorControl()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            this
                .WhenAnyValue(x => x.ViewModel)
                .BindTo(this, x => x.DataContext)
                .DisposeWith(d);

            SkiaCanvas.Events().PaintSurface
                .Subscribe(args => ViewModel?.HandlePaintSurface(args))
                .DisposeWith(d);

            SkiaCanvas.Events().MouseLeftButtonUp
                .Subscribe(args => ViewModel?.HandleMouseLeftButtonUp(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().TouchUp
                .Subscribe(args => ViewModel?.HandleTouchUp(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().StylusUp
                .Subscribe(args => ViewModel?.HandleStylusUp(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().ManipulationDelta
                .Subscribe(args => ViewModel?.HandleManipulationDelta(args))
                .DisposeWith(d);

            SkiaCanvas.Events().MouseWheel
                .Subscribe(args => ViewModel?.HandleMouseWheel(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().MouseDown
                .Subscribe(args => ViewModel?.HandleMouseDown(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().MouseMove
                .Subscribe(args => ViewModel?.HandleMouseMove(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().MouseUp
                .Subscribe(args => ViewModel?.HandleMouseUp(SkiaCanvas, args))
                .DisposeWith(d);

            SkiaCanvas.Events().KeyDown
                .Subscribe(args => ViewModel?.HandleKeyDown(SkiaCanvas, args))
                .DisposeWith(d);

            Locator.Current
                .GetRequiredService<ISubscriber<RenderTransitionEditorCommand>>()
                .Subscribe(_ => SkiaCanvas.InvalidateVisual())
                .DisposeWith(d);

            Locator.Current.GetRequiredService<IPublisher<RenderTransitionEditorCommand>>()
                .Publish(new RenderTransitionEditorCommand());
        });
    }
}