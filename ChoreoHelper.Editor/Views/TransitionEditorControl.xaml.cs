using System.Windows.Input;
using SkiaSharp.Views.Desktop;

namespace ChoreoHelper.Editor.Views;

public partial class TransitionEditorControl
{
    public TransitionEditorControl()
    {
        InitializeComponent();
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
        => ViewModel?.HandlePaintSurface(sender, e);

    private void HandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        => ViewModel?.HandleMouseLeftButtonUp(SkiaCanvas, e);
}