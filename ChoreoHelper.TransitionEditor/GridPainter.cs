﻿using System.Drawing;
using ChoreoHelper.Entities;
using SkiaSharp;

namespace ChoreoHelper.TransitionEditor;

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
        DanceFigureTransition[,] transitions,
        DanceFigure[] figures,
        bool isDanceLoaded,
        SKMatrix transformationMatrix)
    {
        var result = new GridPositions();

        var canvas = surface.Canvas;
        canvas.Clear(Theme.BackgroundColor);
        canvas.SetMatrix(in transformationMatrix);

        if (!isDanceLoaded)
        {
            // return empty result
            return result;
        }

        int figureCount = figures.Length;

        // Define cell sizes
        float cellWidth = 20;
        float cellHeight = 20;

        float headerWidth = (
            from figure in figures
            let paint = GetTextPaintForFigure(figure)
            select paint.MeasureText(figure.Name))
            .Max();

        // Draw vertical headers (figures)
        for (int i = 0; i < figureCount; i++)
        {
            var figureToDraw = figures[i];
            var paint = GetTextPaintForFigure(figureToDraw);

            // Calculate position
            float y = headerWidth + i * cellHeight;
            canvas.Save();
            canvas.Translate(0, y + cellHeight / 2);
            canvas.DrawText(figureToDraw.Name, 0, 0, paint);
            canvas.Restore();

            RectangleF location = new(0, y, headerWidth, cellHeight);
            result.FigureMap.Add(new(location, figureToDraw));
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
            canvas.Translate(x + cellWidth / 2, headerWidth);
            canvas.RotateDegrees(-90);
            canvas.DrawText(figureToDraw.Name, 0, 0, paint);
            canvas.Restore();

            RectangleF location = new(x, 0, cellWidth, headerWidth);
            result.FigureMap.Add(new(location, figureToDraw));
        }

        // Draw grid cells
        for (int col = 0; col < figureCount; col++)
        for (int row = 0; row < figureCount; row++)
        {
            float y = headerWidth + row * cellHeight;
            float x = headerWidth + col * cellWidth;

            // Get the transition value
            var transition = transitions[row, col];
            var distancePaint = GetPaintForDistance(transition.Distance);

            canvas.DrawRect(x, y, cellWidth, cellHeight, distancePaint);

            RectangleF location = new(x, y, cellWidth, cellHeight);
            result.CellMap.Add(new(location, col, row));
        }

        canvas.Restore();
        return result;
    }

    private SKPaint GetPaintForDistance(Entities.Distance distance)
    {
        return distance.Match(
                value => value switch
                {
                    <= 0 => Theme.DistanceUnreachablePaint,
                    <= 1 => Theme.Distance1Paint,
                    > 1 => Theme.Distance2Paint,
                    _ => Theme.DistanceInvalidPaint
                },
                none => Theme.DistanceUnreachablePaint,
                unknown => Theme.DistanceUnreachablePaint);
    }

    private SKPaint GetTextPaintForFigure(DanceFigure figureToDraw)
    {
        var level = figureToDraw.Level;
        var paint = level switch
        {
            _ when level.HasFlag(DanceLevel.Advanced) => Theme.PlatinTextPaint,
            _ when level.HasFlag(DanceLevel.Gold) => Theme.GoldTextPaint,
            _ when level.HasFlag(DanceLevel.Silver) => Theme.SilverTextPaint,
            _ when level.HasFlag(DanceLevel.Bronze) => Theme.BronzeTextPaint,
            _ => Theme.TextPaint
        };
        return paint;
    }

    public void Dispose() => _disposables.Dispose();
}