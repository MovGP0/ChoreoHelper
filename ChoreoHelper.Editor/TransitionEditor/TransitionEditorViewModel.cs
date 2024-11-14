﻿using System.Reactive;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChoreoHelper.Editor.TransitionEditor.Events;
using ChoreoHelper.Entities;
using DynamicData.Binding;
using MessagePipe;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using Splat;

namespace ChoreoHelper.Editor.TransitionEditor;

public sealed class TransitionEditorViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    private GridPainter? _gridPainter;

    private IPublisher<RenderTransitionEditorCommand> RenderTransitionEditorPublisher { get; }

    private TransitionEditorViewModel()
    {
        HostScreen = null!;
        RenderTransitionEditorPublisher = null!;
    }

    public TransitionEditorViewModel(
        IScreen hostScreen,
        IPublisher<RenderTransitionEditorCommand> renderTransitionEditorPublisher)
    {
        HostScreen = hostScreen;
        RenderTransitionEditorPublisher = renderTransitionEditorPublisher;

        this.WhenActivated(disposables =>
        {
            foreach (var behavior in Locator.Current.GetServices<IBehavior<TransitionEditorViewModel>>())
            {
                behavior.Activate(this, disposables);
            }

            _gridPainter = Locator.Current.GetRequiredService<GridPainter>();
        });
    }

    private GridPositions _gridPositions = new();

    public void HandlePaintSurface(SKPaintSurfaceEventArgs e)
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
    public void HandleManipulationDelta(ManipulationDeltaEventArgs args)
    {
        const float panAmount = 1.5f;
        var sx = TransformationMatrix.ScaleX;
        var sy = TransformationMatrix.ScaleY;

        // Pan based on touch translation
        var translationMatrix = SKMatrix.CreateTranslation(
            (float)args.DeltaManipulation.Translation.X * panAmount / sx,
            (float)args.DeltaManipulation.Translation.Y * panAmount / sy);

        // Scale based on touch zoom
        var scaleMatrix = SKMatrix.CreateScale((float)args.DeltaManipulation.Scale.X, (float)args.DeltaManipulation.Scale.Y, 0, 0);

        ApplyTransformation(translationMatrix);
        ApplyTransformation(scaleMatrix);
        Render();
    }

    public void HandleMouseWheel(IInputElement skiaCanvas, MouseWheelEventArgs args)
    {
        var zoomFactor = args.Delta > 0 ? 1.1f : 0.9f;
        var mousePosition = args.GetPosition(skiaCanvas);

        var scaleMatrix = SKMatrix.CreateScale(zoomFactor, zoomFactor, (float)mousePosition.X, (float)mousePosition.Y);

        ApplyTransformation(scaleMatrix);
        Render();
    }

    public void HandleMouseDown(IInputElement skiaCanvas, MouseButtonEventArgs e)
    {
        switch (e.LeftButton)
        {
            case MouseButtonState.Pressed:
                LastMousePosition = e.GetPosition(skiaCanvas);
                skiaCanvas.CaptureMouse();
                break;
        }
    }

    public void HandleMouseMove(IInputElement skiaCanvas, MouseEventArgs e)
    {
        const float panAmount = 1.5f;
        var sx = TransformationMatrix.ScaleX;
        var sy = TransformationMatrix.ScaleY;

        switch (e.LeftButton)
        {
            case MouseButtonState.Pressed:
            {
                if (LastMousePosition.HasValue)
                {
                    var currentPosition = e.GetPosition(skiaCanvas);
                    var delta = currentPosition - LastMousePosition.Value;

                    var translationMatrix = SKMatrix.CreateTranslation(
                        (float)delta.X * panAmount / sx,
                        (float)delta.Y * panAmount / sy);
                    ApplyTransformation(translationMatrix);

                    LastMousePosition = currentPosition;
                    Render();
                }
                break;
            }
        }
    }

    public void HandleMouseUp(IInputElement skiaCanvas, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
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

    public void HandleMouseLeftButtonUp(SKElement canvasElement, MouseButtonEventArgs e)
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
                var (row, col) = (map.Row, map.Column);
                var transition = GetTransition(row, col);
                // TODO: load transition editor dialog
                // TODO: canvasElement.InvalidateVisual(); // Redraw the canvas
                break;
            }
        }

        foreach (var map in _gridPositions.FigureMap)
        {
            var rect = map.ScreenLocation.ToSKRect();
            if (rect.Contains(location))
            {
                var figure = map.DanceFigure;
                // TODO: load figure editor dialog
                break;
            }
        }
    }
}