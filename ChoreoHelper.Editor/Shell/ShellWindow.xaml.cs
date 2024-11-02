using System.Windows;
using ReactiveUI;

namespace ChoreoHelper.Editor.Shell;

public sealed partial class ShellWindow : Window, IViewFor<ShellViewModel>
{
    public ShellWindow()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as ShellViewModel;
    }

    public ShellViewModel? ViewModel { get; set; }
}