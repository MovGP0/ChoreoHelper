using System.Windows.Controls;
using ChoreoHelper.ViewModels;
using ReactiveUI;

namespace ChoreoHelper.Controls;

public partial class OptionalFigureSelection : UserControl, IViewFor<OptionalFigureSelectionViewModel>
{
    public OptionalFigureSelection()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as OptionalFigureSelectionViewModel;
    }

    public OptionalFigureSelectionViewModel? ViewModel { get; set; }
}