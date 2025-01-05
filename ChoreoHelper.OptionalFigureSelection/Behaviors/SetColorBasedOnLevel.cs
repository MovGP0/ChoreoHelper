using System.Windows.Media;
using ChoreoHelper.Entities;

namespace ChoreoHelper.OptionalFigureSelection.Behaviors;

public sealed class SetColorBasedOnLevelBehavior : IBehavior<OptionalFigureSelectionViewModel>
{
    private readonly Dictionary<DanceLevel, Brush> _brushCache;

    public SetColorBasedOnLevelBehavior()
    {
        SolidColorBrush bronzeBrush = new(Color.FromRgb(0xcd, 0x7f, 0x32));
        SolidColorBrush silverBrush = new(Color.FromRgb(0xc0, 0xc0, 0xc0));
        SolidColorBrush goldBrush = new(Color.FromRgb(0xff, 0xd7, 0x00));
        SolidColorBrush platinumBrush = new(Color.FromRgb(0xe5, 0xe4, 0xe2));

        _brushCache = new Dictionary<DanceLevel, Brush>
        {
            { DanceLevel.Bronze, bronzeBrush },
            { DanceLevel.Silver, silverBrush },
            { DanceLevel.Gold, goldBrush },
            { DanceLevel.Advanced, platinumBrush },
            { DanceLevel.BronzeToSilver, bronzeBrush },
            { DanceLevel.BronzeToGold, bronzeBrush },
            { DanceLevel.All, bronzeBrush }
        };

        foreach (var brush in _brushCache.Values)
        {
            if (!brush.IsFrozen)
            {
                brush.Freeze();
            }
        }
    }

    public void Activate(OptionalFigureSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.Level)
            .Select(MapBrushBasedOnLevel)
            .Subscribe(color => viewModel.Color = color)
            .DisposeWith(disposables);
    }

    private Brush MapBrushBasedOnLevel(DanceLevel level)
    {
        return _brushCache.TryGetValue(level, out var brush) 
            ? brush 
            : Brushes.Transparent;
    }
}