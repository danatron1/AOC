using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D3_2019 : Day
    {
        public struct Point
        {
            public int X, Y;
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
            public override bool Equals(object? obj)
            {
                if (obj is not Point) return false;
                Point p = (Point)obj;
                return X == p.X && Y == p.Y;
            }
            public static bool operator ==(Point p1, Point p2) => p1.Equals(p2);
            public static bool operator !=(Point p1, Point p2) => !p1.Equals(p2);
            public int DistanceFromStart() => Math.Abs(X) + Math.Abs(Y); //Manhattan distance
        }
        public struct Line
        {
            public Point Start, End;
            public Line(Point previous, Point current)
            {
                Start = previous;
                End = current;
            }
            public int Plane => Horizontal ? Start.Y : Start.X;
            public int LowerBound => Horizontal ? Math.Min(Start.X, End.X) : Math.Min(Start.Y, End.Y);
            public int UpperBound => Horizontal ? Math.Max(Start.X, End.X) : Math.Max(Start.Y, End.Y);
            public bool Horizontal => Start.Y == End.Y;
            public bool Intersects(Line otherLine)
            {
                if (otherLine.Horizontal == Horizontal) return false;
                if (otherLine.Plane < LowerBound || otherLine.Plane > UpperBound) return false;
                if (Plane < otherLine.LowerBound || Plane > otherLine.UpperBound) return false;
                return true;
            }
            public Point? IntersectionPoint(Line otherLine)
            {
                if (!Intersects(otherLine)) return null;
                if (Horizontal) return new(otherLine.Plane, Plane);
                else return new(Plane, otherLine.Plane);
            }
            public int DistanceFromStart()
            {
                return Math.Min(Start.DistanceFromStart(), End.DistanceFromStart());
            }
        }
        public class Wire
        {
            public Line[] Lines;
            public Wire(string path)
            {
                string[] parts = path.Split(',');
                Lines = new Line[parts.Length];
                Point previous, current = new(0, 0);
                for (int i = 0; i < parts.Length; i++)
                {
                    previous = current;
                    int distance = int.Parse(parts[i][1..]);
                    switch (parts[i][0])
                    {
                        case 'R':
                            current.X += distance;
                            break;
                        case 'L':
                            current.X -= distance;
                            break;
                        case 'U':
                            current.Y += distance;
                            break;
                        case 'D':
                            current.Y -= distance;
                            break;
                    }
                    Lines[i] = new Line(previous, current);
                }
            }
        }
        public override void PartOne()
        {
            Stopwatch sw = new Stopwatch();
            string[] input = GetInputForDay();
            List<Wire> wires = new List<Wire>();
            sw.Start();
            foreach (string item in input)
            {
                wires.Add(new(item));
            }
            int best = int.MaxValue;
            foreach (Line line in wires[0].Lines)
            {
                if (line.DistanceFromStart() >= best) continue;
                foreach (Line crossing in wires[1].Lines)
                {
                    if (crossing.DistanceFromStart() >= best) continue;
                    Point? intersection = crossing.IntersectionPoint(line);
                    if (!intersection.HasValue) continue;
                    if (intersection.Value.DistanceFromStart() < best) best = intersection.Value.DistanceFromStart();
                }
            }
            sw.Stop();
            Console.WriteLine($"Completed in {sw.ElapsedMilliseconds}ms");
            Copy(best);
        }

        public override void Solve() => PartOne();

        public override void PartTwo()
        {
            string[] test = GetInputForDay();
            Copy(test[0]);
            throw new NotImplementedException();
        }
    }
}
