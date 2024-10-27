using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using ChoreoHelper.Editor.Model;
using ChoreoHelper.Entities;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using Splat;

namespace ChoreoHelper.Editor.ViewModels;

public sealed class MainViewModel : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly GridPainter? _gridPainter;

    public MainViewModel()
    {
        if (this.IsInDesignMode())
        {
            // TODO: set design-time data
            return;
        }

        foreach (var behavior in Locator.Current.GetServices<IBehavior<MainViewModel>>())
        {
            behavior.Activate(this, _disposables);
        }

        _gridPainter = Locator.Current.GetRequiredService<GridPainter>();
    }

    private GridPositions _gridPositions = new();

    public void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var isDanceLoaded = SelectedDance is not null
            && Figures.Count > 0
            && Transitions.GetLength(0) == Figures.Count
            && Transitions.GetLength(1) == Figures.Count;

        var result = _gridPainter?.PaintSurface(
            e.Surface,
            e.Info,
            e.RawInfo,
            Transitions,
            Figures.ToArray(),
            isDanceLoaded);

        if (result != null)
        {
            _gridPositions = result;
        }
    }

    public void HandleMouseLeftButtonUp(SKElement canvasElement, MouseButtonEventArgs e)
    {
        var point = e.GetPosition(canvasElement);
        float x = (float)point.X;
        float y = (float)point.Y;

        foreach (var map in _gridPositions.CellMap)
        {
            var rect = map.ScreenLocation;
            if (rect.Contains(x, y))
            {
                var (row, col) = (map.Row, map.Column);
                ToggleCellState(row, col);
                canvasElement.InvalidateVisual(); // Redraw the canvas
                break;
            }
        }

        foreach (var map in _gridPositions.FigureMap)
        {
            var rect = map.ScreenLocation;
            if (rect.Contains(x, y))
            {
                var figure = map.DanceFigure;
                // TODO: handle figure selection
                break;
            }
        }
    }

    /// <summary>
    /// Opening the data file.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit> Open { get; set; } = DisabledCommand.Instance;

    /// <summary>
    /// The path to the data file.
    /// </summary>
    [Reactive]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// The list of the dances
    /// </summary>
    public ObservableCollectionExtended<Dance> Dances { get; } = new ObservableCollectionExtended<Dance>();

    /// <summary>
    /// The currently selected dance
    /// </summary>
    [Reactive]
    public Dance? SelectedDance { get; set; }

    /// <summary>
    /// The list of figures of the selected dance
    /// </summary>
    public IObservableCollection<DanceFigure> Figures { get; } = new ObservableCollectionExtended<DanceFigure>();

    /// <summary>
    /// Grid data representing transitions between figures.
    /// </summary>
    [Reactive]
    public byte[,] Transitions { get; set; } = new byte[0, 0];

    /// <summary>
    /// Cycle through states 0 -> 1 -> 2 -> 0
    /// </summary>
    /// <param name="row">The row of the figure</param>
    /// <param name="col">The column of the figure</param>
    public void ToggleCellState(int row, int col)
        => Transitions[row, col] = (byte)((Transitions[row, col] + 1) % 3);

    /// <summary>
    /// Add a new figure to the selected dance.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit> Add { get; set; } = DisabledCommand.Instance;

    /// <summary>
    /// Save the changes to the data file.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit> Save { get; set; } = DisabledCommand.Instance;

    public void Dispose() => _disposables.Dispose();
}