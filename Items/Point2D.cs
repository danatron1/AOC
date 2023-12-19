using AOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;

public struct Point2D : IEquatable<Point2D>, IEqualityComparer<Point2D>, 
    IAdditionOperators<Point2D, Point2D, Point2D>, ISubtractionOperators<Point2D, Point2D, Point2D>, 
    IMultiplyOperators<Point2D, int, Point2D>, IDivisionOperators<Point2D, int, Point2D>, IComparable<Point2D>,
    IPathfinderNode<Point2D> 
{
    public int X { get; init; }
    public int Y { get; init; }
    public readonly Point2D North => (X, Y + 1);
    public readonly Point2D East => (X + 1, Y);
    public readonly Point2D South => (X, Y - 1);
    public readonly Point2D West => (X - 1, Y);
    public Point2D() : this(0, 0) { }
    public Point2D(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Point2D(string signature)
    {
        string[] split = signature.Trim('(', ')').Split(',');
        X = int.Parse(split[0]);
        Y = int.Parse(split[1]);
    }
    public static bool operator ==(Point2D left, Point2D right) => left.Equals(right);
    public static bool operator !=(Point2D left, Point2D right) => !left.Equals(right);
    public bool Equals(Point2D other) => X == other.X && Y == other.Y;
    public bool Equals(Point2D x, Point2D y) => x.Equals(y);
    public int GetHashCode([DisallowNull] Point2D obj) => HashCode.Combine(obj.X, obj.Y);
    public static implicit operator Point2D((int x, int y) v) => new(v.x, v.y);

    public static Point2D operator +(Point2D left, Point2D right) => (left.X + right.X, left.Y + right.Y);
    public static Point2D operator -(Point2D left, Point2D right) => (left.X - right.X, left.Y -  right.Y);

    public static Point2D operator *(int left, Point2D right) => right * left;
    public static Point2D operator *(Point2D left, int right) => (left.X * right, left.Y * right);
    public static Point2D operator /(Point2D left, int right) => (left.X / right, left.Y / right);

    public readonly IEnumerable<Point2D> Surrounding(bool includeSelf = false)
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
    public readonly IEnumerable<Point2D> Adjacent()
    {
        yield return North;
        yield return East;
        yield return South;
        yield return West;
    }
    internal readonly IEnumerable<(Point2D point, Direction dir)> AdjacentWithDirection()
    {
        yield return (North, Direction.North);
        yield return (East, Direction.East);
        yield return (South, Direction.South);
        yield return (West, Direction.West);
    }
    public int ManhattanDistanceTo(Point2D other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    public double PythagorasDistanceTo(Point2D other)
    {
        double x = Math.Abs(other.X - X);
        double y = Math.Abs(other.Y - Y);
        return Math.Sqrt(x * x + y * y);
    }
    public override string ToString() => $"({X}, {Y})";
    internal Point2D NextIn(Direction? dir, int distance = 1)
    {
        return dir switch
        {
            Direction.North => (X, Y + distance),
            Direction.East => (X + distance, Y),
            Direction.South => (X, Y - distance),
            Direction.West => (X - distance, Y),
            _ => this,
        };
    }
    public override bool Equals(object obj) => obj is Point2D && Equals((Point2D)obj);

    public override int GetHashCode() => GetHashCode(this);

    public readonly int CompareTo(Point2D other)
    {
        if (Y == other.Y) return X.CompareTo(other.X);
        return -Y.CompareTo(other.Y);
    }
    public Direction? DirectionFrom(Point2D source)
    {
        if (X == source.X)
        {
            if (Y < source.Y) return Direction.South;
            if (Y > source.Y) return Direction.North;
        }
        if (Y == source.Y)
        {
            if (X < source.X) return Direction.West;
            if (X > source.X) return Direction.East;
        }
        return null;
    }
}
