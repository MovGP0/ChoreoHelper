using System.Windows;

namespace ChoreoHelper.MainWindow;

public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as MainWindowViewModel;
    }

    public MainWindowViewModel? ViewModel { get; set; }
}