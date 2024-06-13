using System.Windows.Controls;
using ChoreoHelper.ViewModels;
using ReactiveUI;

namespace ChoreoHelper.Controls;

public partial class LevelSelection : UserControl, IViewFor<LevelSelectionViewModel>
{
    public LevelSelection()
    {
        InitializeComponent();
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as LevelSelectionViewModel;
    }

    public LevelSelectionViewModel? ViewModel { get; set; }
}