using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class Food
{
    public Point Position { get; private set; }

    public Food()
    {
        Position = new Point(0, 0); // Replace Point.Zero with explicit Point creation
    }

    public void Spawn(GameGrid grid, List<Point> occupiedPositions)
    {
        Position = grid.GetRandomEmptyCell(occupiedPositions);
    }
}
