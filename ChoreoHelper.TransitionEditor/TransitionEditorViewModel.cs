using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChoreoHelper.EditFigure;
using ChoreoHelper.Entities;
using ChoreoHelper.Transition;
using ChoreoHelper.TransitionEditor.Events;
using DynamicData.Binding;
using DynamicData.Kernel;
using JetBrains.Annotations;
using ReactiveUI.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace ChoreoHelper.TransitionEditor;

public sealed class TransitionEditorViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    private GridPainter GridPainter { get; }
    private IPublisher<RenderTransitionEditorCommand> RenderTransitionEditorPublisher { get; }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Itself)]
    private TransitionEditorViewModel()
    {
        HostScreen = null!;
        RenderTransitionEditorPublisher = null!;
        GridPainter = null!;

        if (this.IsInDesignMode())
        {
            IsEditViewOpen = true;
            EditViewModel = new TransitionViewModel();
        }
    }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Itself)]
    public TransitionEditorViewModel(
        IScreen hostScreen,
        IPublisher<RenderTransitionEditorCommand> renderTransitionEditorPublisher,
        GridPainter gridPainter)
    {
        HostScreen = hostScreen;
        RenderTransitionEditorPublisher = renderTransitionEditorPublisher;
        GridPainter = gridPainter;

        this.WhenActivated(ActivateBehaviors);
    }

    private void ActivateBehaviors(CompositeDisposable disposables)
    {
        foreach (var behavior in Locator.Current.GetServices<IBehavior<TransitionEditorViewModel>>())
        {
            behavior.Activate(this, disposables);
        }
    }

    private GridPositions _gridPositions = new();

    public void HandlePaintSurface(SKPaintSurfaceEventArgs e)
    {
        var isDanceLoaded = SelectedDance is not null
            && Figures.Count > 0
            && Transitions.GetLength(0) == Figures.Count
            && Transitions.GetLength(1) == Figures.Count;

        var result = GridPainter?.PaintSurface(
            e.Surface,
            e.Info,
            e.RawInfo,
            Transitions,
            Figures.ToArray(),
            isDanceLoaded,
            TransformationMatrix);

        if (result != null)
        {
            _gridPositions = result;
        }
    }

    /// <summary>
    /// The list of the dances
    /// </summary>
    public IObservableCollection<Dance> Dances { get; } = new ObservableCollectionExtended<Dance>();

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
    public DanceFigureTransition[,] Transitions { get; set; } = new DanceFigureTransition[0, 0];

    /// <summary>
    /// Cycle through states 0 -> 1 -> 2 -> 0
    /// </summary>
    /// <param name="row">The row of the figure</param>
    /// <param name="col">The column of the figure</param>
    private DanceFigureTransition GetTransition(int row, int col) => Transitions[col, row];

    public ViewModelActivator Activator { get; } = new();

    public string? UrlPathSegment { get; } = "transition_editor";

    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> ResetZoom { get; set; } = DisabledCommand.Instance;

    /// <summary>
    /// Matrix that gets applied for zoom/pan operations.
    /// </summary>
    public SKMatrix TransformationMatrix { get; set; } = SKMatrix.CreateIdentity();

    [Reactive]
    public bool IsEditViewOpen { get; set; } = false;

    [Reactive]
    public object? EditViewModel { get; set; }

    /// <summary>
    /// Keeps the mouse position for dragging operations. <c>null</c> when no drag is in progress.
    /// </summary>
    private Point? LastMousePosition { get; set; }

    /// <summary>
    /// Applies the transformation matrix and forces a re-render.
    /// </summary>
    private void ApplyTransformation(SKMatrix newMatrix)
    {
        const float maxZoomFactor = 5f;
        var newTransformationMatrix = SKMatrix.Concat(TransformationMatrix, newMatrix);
        if (newTransformationMatrix is { ScaleX: < maxZoomFactor, ScaleY: < maxZoomFactor })
        {
            TransformationMatrix = newTransformationMatrix;
        }
    }

    private void Render() => RenderTransitionEditorPublisher.Publish(new RenderTransitionEditorCommand());

    /// <summary>
    /// Touch Gesture for Zoom and Pan
    /// </summary>
    public void HandleManipulationDelta(SKElement skiaCanvas, ManipulationDeltaEventArgs args)
    {
        // Get the DPI scaling factors
        var dpiScaleX = VisualTreeHelper.GetDpi(skiaCanvas).DpiScaleX;
        var dpiScaleY = VisualTreeHelper.GetDpi(skiaCanvas).DpiScaleY;

        var sx = TransformationMatrix.ScaleX;
        var sy = TransformationMatrix.ScaleY;

        // Pan based on touch translation
        var translationMatrix = SKMatrix.CreateTranslation(
            (float)(args.DeltaManipulation.Translation.X * dpiScaleX) / sx,
            (float)(args.DeltaManipulation.Translation.Y * dpiScaleY) / sy);

        // Scale based on touch zoom
        var scaleMatrix = SKMatrix.CreateScale((float)args.DeltaManipulation.Scale.X, (float)args.DeltaManipulation.Scale.Y, 0, 0);

        ApplyTransformation(translationMatrix);
        ApplyTransformation(scaleMatrix);
        Render();
    }

    public void HandleMouseWheel(SKElement skiaCanvas, MouseWheelEventArgs args)
    {
        var zoomFactor = args.Delta > 0 ? 1.1f : 0.9f;
        var mousePosition = args.GetPosition(skiaCanvas);

        var scaleMatrix = SKMatrix.CreateScale(
            zoomFactor,
            zoomFactor,
            (float)mousePosition.X,
            (float)mousePosition.Y);

        ApplyTransformation(scaleMatrix);
        Render();
    }

    public void HandleMouseDown(IInputElement skiaCanvas, MouseButtonEventArgs args)
    {
        switch (args.LeftButton)
        {
            case MouseButtonState.Pressed:
                LastMousePosition = args.GetPosition(skiaCanvas);
                skiaCanvas.CaptureMouse();
                break;
        }
    }

    public void HandleMouseMove(SKElement skiaCanvas, MouseEventArgs args)
    {
        // Get the DPI scaling factors
        var dpiScaleX = VisualTreeHelper.GetDpi(skiaCanvas).DpiScaleX;
        var dpiScaleY = VisualTreeHelper.GetDpi(skiaCanvas).DpiScaleY;

        var sx = TransformationMatrix.ScaleX;
        var sy = TransformationMatrix.ScaleY;

        switch (args.LeftButton)
        {
            case MouseButtonState.Pressed:
            {
                if (LastMousePosition.HasValue)
                {
                    var currentPosition = args.GetPosition(skiaCanvas);
                    var delta = currentPosition - LastMousePosition.Value;

                    var translationMatrix = SKMatrix.CreateTranslation(
                        (float)(delta.X * dpiScaleX) / sx,
                        (float)(delta.Y * dpiScaleY) / sy);
                    ApplyTransformation(translationMatrix);

                    LastMousePosition = currentPosition;
                    Render();
                }
                break;
            }
        }
    }

    public void HandleMouseUp(SKElement skiaCanvas, MouseButtonEventArgs args)
    {
        if (args.LeftButton == MouseButtonState.Released)
        {
            var newPosition = args.GetPosition(skiaCanvas);
            if (LastMousePosition == newPosition)
            {
                HandleMouseClick(skiaCanvas, args);
            }

            LastMousePosition = null;
            skiaCanvas.ReleaseMouseCapture();
        }
    }

    public void HandleKeyDown(SKElement skiaCanvas, KeyEventArgs args)
    {
        const float panAmount = 10f;
        var sx = TransformationMatrix.ScaleX;
        var sy = TransformationMatrix.ScaleY;

        SKMatrix transformation = args.Key switch
        {
            Key.Up => SKMatrix.CreateTranslation(0, -panAmount / sy),
            Key.Down => SKMatrix.CreateTranslation(0, panAmount / sy),
            Key.Left => SKMatrix.CreateTranslation(-panAmount / sx, 0),
            Key.Right => SKMatrix.CreateTranslation(panAmount / sx, 0),
            Key.Add or Key.OemPlus => SKMatrix.CreateScale(
                1.1f,
                1.1f,
                (float)skiaCanvas.ActualWidth / 2f,
                (float)skiaCanvas.ActualHeight / 2f),
            Key.Subtract or Key.OemMinus => SKMatrix.CreateScale(
                0.9f,
                0.9f,
                (float)skiaCanvas.ActualWidth / 2f,
                (float)skiaCanvas.ActualHeight / 2f),
            _ => SKMatrix.CreateIdentity()
        };

        ApplyTransformation(transformation);
        Render();
    }

    public void HandleTouchUp(SKElement canvasElement, TouchEventArgs args)
    {
        // Get the DPI scaling factors
        var dpiScaleX = VisualTreeHelper.GetDpi(canvasElement).DpiScaleX;
        var dpiScaleY = VisualTreeHelper.GetDpi(canvasElement).DpiScaleY;

        // get adjusted touch position
        var point = args.GetTouchPoint(canvasElement).Position;
        var adjustedPoint = new Point(point.X * dpiScaleX, point.Y * dpiScaleY);
        var skPoint = adjustedPoint.ToSKPoint();
        var touchLocation = TransformationMatrix.Invert().MapPoint(skPoint);

        SelectElementAtLocation(touchLocation);
    }

    public void HandleStylusUp(SKElement canvasElement, StylusEventArgs args)
    {
        // Get the DPI scaling factors
        var dpiScaleX = VisualTreeHelper.GetDpi(canvasElement).DpiScaleX;
        var dpiScaleY = VisualTreeHelper.GetDpi(canvasElement).DpiScaleY;

        // get adjusted stylus position
        var point = args.GetStylusPoints(canvasElement).First().ToPoint();
        var adjustedPoint = new Point(point.X * dpiScaleX, point.Y * dpiScaleY);
        var skPoint = adjustedPoint.ToSKPoint();
        var stylusLocation = TransformationMatrix.Invert().MapPoint(skPoint);

        SelectElementAtLocation(stylusLocation);
    }

    private void HandleMouseClick(SKElement canvasElement, MouseButtonEventArgs e)
    {
        // Get the DPI scaling factors
        var dpiScaleX = VisualTreeHelper.GetDpi(canvasElement).DpiScaleX;
        var dpiScaleY = VisualTreeHelper.GetDpi(canvasElement).DpiScaleY;

        // get adjusted mouse position
        var point = e.GetPosition(canvasElement);
        var adjustedPoint = new Point(point.X * dpiScaleX, point.Y * dpiScaleY);
        var skPoint = adjustedPoint.ToSKPoint();
        var mouseLocation = TransformationMatrix.Invert().MapPoint(skPoint);

        SelectElementAtLocation(mouseLocation);
    }

    private void SelectElementAtLocation(SKPoint location)
    {
        foreach (var map in _gridPositions.CellMap)
        {
            var rect = map.ScreenLocation.ToSKRect();
            if (rect.Contains(location))
            {
                var transition = GetTransition(map.Row, map.Column);
                ShowTransitionEditor(transition);
                return;
            }
        }

        foreach (var map in _gridPositions.FigureMap)
        {
            var rect = map.ScreenLocation.ToSKRect();
            if (rect.Contains(location))
            {
                var figure = map.DanceFigure;
                ShowFigureEditor(figure);
                return;
            }
        }

        IsEditViewOpen = false;
    }

    private void ShowFigureEditor(DanceFigure figure)
    {
        var figureViewModel = new EditFigureViewModel
        {
            Name = figure.Name,
            Hash = figure.Dance.Name + '|' + figure.Name
        };

        var level = figureViewModel.Levels
            .FirstOrOptional(l => l.Level == figure.Level);

        figureViewModel.Level = level.HasValue
            ? level.Value
            : figureViewModel.Level = figureViewModel.Levels.First();

        var restriction = figureViewModel.Restrictions
            .FirstOrOptional(r => r.Restriction == figure.Restriction);
        
        figureViewModel.Restriction = restriction.HasValue
            ? restriction.Value
            : figureViewModel.Restrictions.First();

        EditViewModel = figureViewModel;
        IsEditViewOpen = true;
    }

    private void ShowTransitionEditor(DanceFigureTransition transition)
    {
        var transitionViewModel = new TransitionViewModel
        {
            FromFigureName = transition.Source.Name,
            ToFigureName = transition.Target.Name
        };

        var result = transitionViewModel.Distances
            .FirstOrOptional(d => DistanceComparer.Default.Equals(d.Distance, transition.Distance));

        transitionViewModel.SelectedDistance = result.HasValue
            ? result.Value
            : transitionViewModel.Distances.First();

        var rest = transitionViewModel.Restrictions
            .FirstOrOptional(r => r.Restriction == transition.Restriction);

        transitionViewModel.SelectedRestriction = rest.HasValue
            ? rest.Value
            :  transitionViewModel.Restrictions.First();

        EditViewModel = transitionViewModel;
        IsEditViewOpen = true;
    }
}