using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;

public struct Point2D : IEquatable<Point2D>, IEqualityComparer<Point2D>
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }
    public int X { get; init; }
    public int Y { get; init; }
    public Point2D() : this(0, 0) { }
    public Point2D(int x, int y)
    {
        X = x;
        Y = y;
    }
    public bool Equals(Point2D other) => X == other.X && Y == other.Y;
    public bool Equals(Point2D x, Point2D y) => x.Equals(y);
    public int GetHashCode([DisallowNull] Point2D obj) => HashCode.Combine(obj.X, obj.Y);
    public static implicit operator Point2D((int x, int y) v) => new(v.x, v.y);
    public IEnumerable<Point2D> Surrounding(bool includeSelf = false)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0 && !includeSelf) continue;
                yield return (i+X, j+Y);
            }
        }
    }
    public override string ToString() => $"({X}, {Y})";
    internal Point2D NextIn(Direction dir)
    {
        return dir switch
        {
            Direction.North => new Point2D(X, Y + 1),
            Direction.East => new Point2D(X + 1, Y),
            Direction.South => new Point2D(X, Y - 1),
            Direction.West => new Point2D(X - 1, Y),
            _ => new Point2D(X, Y),
        };
    }

    internal static Direction DirectionFromChar(char c)
    {
        c = char.ToUpper(c);
        switch(c)
        {
            case 'U': //up
            case 'N': return Direction.North;
            case 'R': //right
            case 'E': return Direction.East;
            case 'D': //down
            case 'S': return Direction.South;
            case 'L': //left
            case 'W': return Direction.West;
        }
        throw new NotImplementedException($"The case for char {c} is not handled.");
    }
}
