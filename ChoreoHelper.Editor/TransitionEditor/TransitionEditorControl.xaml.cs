using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using SkiaSharp.Views.Desktop;

namespace ChoreoHelper.Editor.Views;

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

            Observable.FromEventPattern<SKPaintSurfaceEventArgs>(
                    eh => SkiaCanvas.PaintSurface += eh,
                    eh => SkiaCanvas.PaintSurface -= eh)
                .Subscribe(args =>
                {
                    ViewModel?.HandlePaintSurface(args.Sender ?? this, args.EventArgs);
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
        });
    }
}