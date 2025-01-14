using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

public class Snake
{
    private List<Point> _body;
    private Point _direction;
    private bool _shouldGrow;

    public Snake(Point startPosition)
    {
        _body = new List<Point> { startPosition };
        _direction = new Point(1, 0);
        _shouldGrow = false;
    }

    public IReadOnlyList<Point> Body => _body;
    public Point Head => _body[0];

    public void SetDirection(Point newDirection)
    {
        if (_direction.X + newDirection.X != 0 || _direction.Y + newDirection.Y != 0)
            _direction = newDirection;
    }

    public void Move()
    {
        var newHead = new Point(Head.X + _direction.X, Head.Y + _direction.Y);
        _body.Insert(0, newHead);
        
        if (!_shouldGrow)
            _body.RemoveAt(_body.Count - 1);
        _shouldGrow = false;
    }

    public void Grow()
    {
        _shouldGrow = true;
    }

    public bool CollidesWith(Point position)
    {
        return _body.Skip(1).Contains(position);
    }
}
