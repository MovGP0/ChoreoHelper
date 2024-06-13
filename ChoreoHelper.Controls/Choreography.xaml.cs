using System.Windows.Controls;
using ChoreoHelper.ViewModels;
using ReactiveUI;

namespace ChoreoHelper.Controls;

public partial class Choreography : UserControl, IViewFor<ChoreographyViewModel>
{
    public Choreography()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as ChoreographyViewModel;
    }

    public ChoreographyViewModel? ViewModel { get; set; }
}