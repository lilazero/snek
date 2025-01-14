using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class GameGrid
{
    public int Width { get; }
    public int Height { get; }
    public int CellSize { get; }

    public GameGrid(int width, int height, int cellSize)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
    }

    public bool IsOutOfBounds(Point position)
    {
        return position.X < 0 || position.X >= Width || position.Y < 0 || position.Y >= Height;
    }

    public Point GetRandomEmptyCell(List<Point> occupiedPositions)
    {
        var random = new Random();
        Point position;
        do
        {
            position = new Point(random.Next(Width), random.Next(Height));
        } while (occupiedPositions.Contains(position));
        return position;
    }
}
