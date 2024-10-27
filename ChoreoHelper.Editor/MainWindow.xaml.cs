using System.Windows;
using System.Windows.Input;
using ChoreoHelper.Editor.ViewModels;
using ReactiveUI;
using SkiaSharp.Views.Desktop;

namespace ChoreoHelper.Editor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IViewFor<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
        => ViewModel?.HandlePaintSurface(sender, e);

    private void HandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        => ViewModel?.HandleMouseLeftButtonUp(SkiaCanvas, e);

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set
        {
            if (value is MainViewModel viewModel)
            {
                ViewModel = viewModel;
                return;
            }

            ViewModel = null;
        }
    }

    public MainViewModel? ViewModel { get; set; }
}