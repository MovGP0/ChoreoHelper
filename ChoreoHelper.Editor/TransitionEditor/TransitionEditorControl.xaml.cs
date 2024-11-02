using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using ChoreoHelper.Editor.TransitionEditor.Events;
using MessagePipe;
using ReactiveUI;
using SkiaSharp.Views.Desktop;
using Splat;

namespace ChoreoHelper.Editor.TransitionEditor;

public partial class TransitionEditorControl
{
    public TransitionEditorControl()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(
                eh => MainGrid.SizeChanged += eh,
                eh => MainGrid.SizeChanged -= eh)
                .Subscribe(args => OnGridSizeChanged())
                .DisposeWith(d);

            this
                .WhenAnyValue(x => x.ViewModel)
                .BindTo(this, x => x.DataContext)
                .DisposeWith(d);

            Observable.FromEventPattern<SKPaintSurfaceEventArgs>(
                    eh => SkiaCanvas.PaintSurface += eh,
                    eh => SkiaCanvas.PaintSurface -= eh)
                .Subscribe(args =>
                {
                    ViewModel?.HandlePaintSurface(args.EventArgs);
                })
                .DisposeWith(d);

            Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                eh => SkiaCanvas.MouseLeftButtonUp += eh,
                eh => SkiaCanvas.MouseLeftButtonDown -= eh)
                .Subscribe(args =>
                {
                    ViewModel?.HandleMouseLeftButtonUp(SkiaCanvas, args.EventArgs);
                })
                .DisposeWith(d);

            Locator.Current
                .GetRequiredService<ISubscriber<RenderTransitionEditorCommand>>()
                .Subscribe(_ => SkiaCanvas.InvalidateVisual())
                .DisposeWith(d);

            Locator.Current.GetRequiredService<IPublisher<RenderTransitionEditorCommand>>()
                .Publish(new RenderTransitionEditorCommand());
        });
    }

    private void OnGridSizeChanged()
    {
        SkiaCanvas.Height = SkiaCanvasRow.ActualHeight - (SkiaCanvas.Margin.Top + SkiaCanvas.Margin.Bottom);
    }
}