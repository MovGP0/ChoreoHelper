using System.Reactive.Disposables;
using ChoreoHelper.Editor.Model;
using ChoreoHelper.Entities;
using SkiaSharp;

namespace ChoreoHelper.Editor.ViewModels;

public sealed class GridPainter : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private Theme Theme { get; }

    public GridPainter(Theme theme)
    {
        Theme = theme;
        _disposables.Add(Theme);
    }

    /// <summary>
    /// Draws the grid of the dance step transitions to the surface.
    /// </summary>
    public GridPositions PaintSurface(
        SKSurface surface,
        SKImageInfo info,
        SKImageInfo rawInfo,
        byte[,] transitions,
        DanceFigure[] figures,
        bool isDanceLoaded)
    {
        var result = new GridPositions();

        var canvas = surface.Canvas;
        canvas.Clear(Theme.BackgroundColor);

        if (!isDanceLoaded)
        {
            // return empty result
            return result;
        }

        int figureCount = figures.Length;

        // Define cell sizes
        float cellWidth = 100;
        float cellHeight = 30;
        float headerHeight = 100;
        float headerWidth = 200;

        // Draw vertical headers (figures)
        for (int i = 0; i < figureCount; i++)
        {
            var figureToDraw = figures[i];
            var paint = GetTextPaintForFigure(figureToDraw);

            // Calculate position
            float y = headerHeight + i * cellHeight;
            canvas.Save();
            canvas.Translate(headerWidth, y + cellHeight / 2);
            canvas.DrawText(figureToDraw.Name, 0, 0, paint);
            canvas.Restore();

            // TODO: Store the screen location of the figure
            // RectangleF location = new(...);
            // result.FigureMap.Add(new(location, figureToDraw));
        }

        // Draw horizontal headers (figures)
        for (int j = 0; j < figureCount; j++)
        {
            var figureToDraw = figures[j];
            var paint = GetTextPaintForFigure(figureToDraw);

            // Calculate position
            float x = headerWidth + j * cellWidth;

            // Draw figure name vertically
            canvas.Save();
            canvas.Translate(x + cellWidth / 2, headerHeight);
            canvas.RotateDegrees(-90);
            canvas.DrawText(figureToDraw.Name, 0, 0, paint);
            canvas.Restore();

            // TODO: Store the screen location of the figure
            // RectangleF location = new(...);
            // result.FigureMap.Add(new(location, figureToDraw));
        }

        // Draw grid cells
        for (int i = 0; i < figureCount; i++)
        for (int j = 0; j < figureCount; j++)
        {
            float x = headerWidth + j * cellWidth;
            float y = headerHeight + i * cellHeight;

            // Get the transition value
            byte value = transitions[i, j];

            SKPaint distancePaint = value switch
            {
                0 => Theme.DistanceUnreachablePaint,
                1 => Theme.Distance1Paint,
                2 => Theme.Distance2Paint,
                _ => Theme.DistanceInvalidPaint
            };

            canvas.DrawRect(x, y, cellWidth, cellHeight, distancePaint);
            canvas.DrawRect(x, y, cellWidth, cellHeight, Theme.BorderPaint);

            // TODO: Store the screen location of the cell
            // RectangleF location = new(...);
            // result.CellMap.Add(new(location, row, column));
        }

        return result;
    }

    private SKPaint GetTextPaintForFigure(DanceFigure figureToDraw)
    {
        var paint = figureToDraw.Level switch
        {
            DanceLevel.Undefined => Theme.TextPaint,
            DanceLevel.Bronze => Theme.BronzeTextPaint,
            DanceLevel.Silver => Theme.SilverTextPaint,
            DanceLevel.Gold => Theme.GoldTextPaint,
            DanceLevel.Advanced => Theme.PlatinTextPaint,
            _ => Theme.TextPaint
        };
        return paint;
    }

    public void Dispose() => _disposables.Dispose();
}