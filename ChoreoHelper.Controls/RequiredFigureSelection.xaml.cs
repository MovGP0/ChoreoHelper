using System.Windows.Controls;
using ChoreoHelper.ViewModels;
using ReactiveUI;

namespace ChoreoHelper.Controls;

public partial class RequiredFigureSelection : UserControl, IViewFor<RequiredFigureSelectionViewModel>
{
    public RequiredFigureSelection()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as RequiredFigureSelectionViewModel;
    }

    public RequiredFigureSelectionViewModel? ViewModel { get; set; }
}