using AOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items.Geometry
{
    public struct Point3D : IEquatable<Point3D>, IEqualityComparer<Point3D>,
    IAdditionOperators<Point3D, Point3D, Point3D>, ISubtractionOperators<Point3D, Point3D, Point3D>,
    IMultiplyOperators<Point3D, int, Point3D>, IDivisionOperators<Point3D, int, Point3D>, IComparable<Point3D>,
    IPathfinderNode<Point3D>
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Z { get; init; }
        public readonly Point3D Up => (X, Y, Z + 1);
        public readonly Point3D North => (X, Y + 1, Z);
        public readonly Point3D East => (X + 1, Y, Z);
        public readonly Point3D South => (X, Y - 1, Z);
        public readonly Point3D West => (X - 1, Y, Z);
        public readonly Point3D Down => (X, Y, Z - 1);
        public Point3D() : this(0, 0, 0) { }
        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Point3D(string signature)
        {
            string[] split = signature.Trim('(', ')').Split(',');
            X = int.Parse(split[0]);
            Y = int.Parse(split[1]);
            Z = int.Parse(split[2]);
        }
        public override readonly string ToString() => $"({X}, {Y}, {Z})";
        public static bool operator ==(Point3D left, Point3D right) => left.Equals(right);
        public static bool operator !=(Point3D left, Point3D right) => !left.Equals(right);
        public readonly bool Equals(Point3D other) => X == other.X && Y == other.Y && Z == other.Z;
        public readonly bool Equals(Point3D a, Point3D b) => a.Equals(b);
        public readonly int GetHashCode([DisallowNull] Point3D obj) => HashCode.Combine(obj.X, obj.Y, obj.Z);

        public static implicit operator Point3D((int x, int y, int z) v) => new(v.x, v.y, v.z);
        public static implicit operator Point3D(Point2D v) => new(v.X, v.Y, 0);
        public readonly Point2D ToPoint2D() => (X, Y);
        public static Point3D operator +(Point3D left, Point3D right) => (left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Point3D operator -(Point3D left, Point3D right) => (left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        public static Point3D operator *(Point3D left, int right) => (left.X * right, left.Y * right, left.Z * right);
        public static Point3D operator /(Point3D left, int right) => (left.X / right, left.Y / right, left.Z / right);
        public readonly int CompareTo(Point3D other)
        {
            if (Z != other.Z) return Z.CompareTo(other.Z);
            if (Y != other.Y) return -Y.CompareTo(other.Y);
            return X.CompareTo(other.X);
        }
        public readonly IEnumerable<Point3D> Adjacent()
        {
            yield return Up;
            yield return North;
            yield return East;
            yield return South;
            yield return West;
            yield return Down;
        }
        internal readonly IEnumerable<(Point3D point, Direction3D dir)> AdjacentWithDirection()
        {
            yield return (Up, Direction3D.Up);
            yield return (North, Direction3D.North);
            yield return (East, Direction3D.East);
            yield return (South, Direction3D.South);
            yield return (West, Direction3D.West);
            yield return (Down, Direction3D.Down);
        }
        public readonly IEnumerable<Point3D> Surrounding(bool includeSelf = false)
        {
            for (int z = -1; z <= 1; z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (z == 0 && y == 0 && x == 0 && !includeSelf) continue;
                        yield return (X + x, Y - y, Z + z);
                    }
                }
            }
        }
        public readonly int ManhattanDistanceTo(Point3D other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);
        public readonly double PythagorasDistanceTo(Point3D other)
        {
            double x = Math.Abs(other.X - X);
            double y = Math.Abs(other.Y - Y);
            double z = Math.Abs(other.Z - Z);
            return Math.Sqrt(x * x + y * y + z * z);
        }
        internal readonly Point3D NextIn(Direction3D? dir, int distance = 1)
        {
            return dir switch
            {
                Direction3D.Up => (X, Y, Z + distance),
                Direction3D.North => (X, Y + distance, Z),
                Direction3D.East => (X + distance, Y, Z),
                Direction3D.South => (X, Y - distance, Z),
                Direction3D.West => (X - distance, Y, Z),
                Direction3D.Down => (X, Y, Z - distance),
                _ => this,
            };
        }
        public override int GetHashCode() => GetHashCode(this);
        public static bool operator <(Point3D left, Point3D right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Point3D left, Point3D right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Point3D left, Point3D right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Point3D left, Point3D right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override bool Equals(object? obj) => obj is Point3D ddd && Equals(ddd);
    }
}
