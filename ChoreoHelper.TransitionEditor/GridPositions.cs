﻿using System.Drawing;
using ChoreoHelper.Entities;

namespace ChoreoHelper.TransitionEditor;

public sealed class GridPositions
{
    /// <summary>
    /// Stores the screen location of a given cell of the grid.
    /// </summary>
    public List<(RectangleF ScreenLocation, int Row, int Column)> CellMap { get; } = new();

    /// <summary>
    /// Stores the location of a given dance figure of the grid.
    /// </summary>
    public List<(RectangleF ScreenLocation, DanceFigure DanceFigure)> FigureMap { get; } = new();
}