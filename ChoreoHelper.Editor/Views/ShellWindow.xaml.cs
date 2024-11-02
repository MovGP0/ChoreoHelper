using System.Windows;
using ChoreoHelper.Editor.ViewModels;
using ReactiveUI;

namespace ChoreoHelper.Editor.Views;

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