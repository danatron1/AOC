using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D15_2021 : Day
    {
        struct Point
        {
            public int X;
            public int Y;
            public static int MaximumX = int.MaxValue;
            public static int MaximumY = int.MaxValue;
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
            public Point[] Neighbours()
            {
                return new Point[]
                {
                    new Point(X + 1, Y),
                    new Point(X - 1, Y),
                    new Point(X, Y + 1),
                    new Point(X, Y - 1)
                };
            }

            internal bool InRange(int minX = int.MinValue, int maxX = int.MinValue, int minY = int.MinValue, int maxY = int.MinValue)
            {
                if (minX == int.MinValue) minX = 0;
                if (maxX == int.MinValue) maxX = MaximumX;
                if (minY == int.MinValue) minY = minX;
                if (maxY == int.MinValue) maxY = MaximumY;
                return X >= minX && X <= maxX && Y >= minY && Y <= maxY;
            }
            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                if (obj is not Point) return false;
                Point p = (Point)obj;
                return X == p.X && Y == p.Y;
            }
            public static bool operator ==(Point p1, Point p2) => p1.Equals(p2);
            public static bool operator !=(Point p1, Point p2) => !p1.Equals(p2);
        }
        public override void PartOne()
        {
            int[,] input = GetInputForDay2D<int>();
            Point start = new Point(0, 0);
            PriorityQueue<Point, int> frontier = new PriorityQueue<Point, int>();
            frontier.Enqueue(start, 0);

            Dictionary<Point, Point?> cameFrom = new Dictionary<Point, Point?>();
            Dictionary<Point, int> costSoFar = new Dictionary<Point, int>();
            cameFrom[start] = null;
            costSoFar[start] = 0;

            Point.MaximumX = input.GetLength(0) - 1;
            Point.MaximumY = input.GetLength(1) - 1;
            Point end = new Point(Point.MaximumX, Point.MaximumY);
            while (frontier.Count > 0)
            {
                Point front = frontier.Dequeue();
                if (front == end) break;
                foreach (Point next in front.Neighbours())
                {
                    if (!next.InRange()) continue;
                    int cost = input[next.X, next.Y] + costSoFar[front];
                    if (costSoFar.ContainsKey(next) && costSoFar[next] <= cost) continue;
                    cameFrom.Add(next, front);
                    frontier.Enqueue(next, cost);
                    costSoFar[next] = cost;
                }
            }
            Copy(costSoFar[end]);
        }

        public override void PartTwo()
        {
            #region Initialise Inputs
            int[,] input = GetInputForDay2D<int>();
            Point.MaximumX = 5 * input.GetLength(0) - 1; //limits of 499 for part 2
            Point.MaximumY = 5 * input.GetLength(1) - 1;
            Point start = new Point(0, 0);
            Point end = new Point(Point.MaximumX, Point.MaximumY);
            Stopwatch sw = new Stopwatch();
            sw.Start(); //spoiler alert; it takes about 10 seconds
            #endregion
            #region Create lists needed for Dijkstra
            PriorityQueue<Point, int> unexplored = new PriorityQueue<Point, int>();
            Dictionary<Point, Point?> cameFrom = new Dictionary<Point, Point?>(); //unnecessary for AOC puzzle
            Dictionary<Point, int> riskToReach = new Dictionary<Point, int>();
            unexplored.Enqueue(start, 0); //begin at start with a risk of 0
            cameFrom[start] = null; //when tracing back a route, null = finished.
            riskToReach[start] = 0; //start node not counted towards risk
            #endregion
            #region Actually doing the pathfinding
            while (unexplored.Count > 0)
            {
                Point front = unexplored.Dequeue(); //look at the next lowest risk unexplored node
                if (front == end) break; //break if found end
                foreach (Point next in front.Neighbours()) //.Neighbours() gets the 4 adjacent points
                {
                    if (!next.InRange()) continue; //.InRange() checks if it's in the 0-499 range (the cave's size)
                    int cost = GetCostAt(next) + riskToReach[front]; //GetCostAt() gives the cost at a position
                    if (riskToReach.ContainsKey(next) && riskToReach[next] <= cost) continue; //no point exploring if I've got here more efficiently before
                    cameFrom[next] = front; //unnecessary for this particular puzzle (but would allow me to trace back a route)
                    unexplored.Enqueue(next, cost); //explore the newly descovered node later
                    riskToReach[next] = cost; //log how much it cost to get there
                }
            }
            #endregion
            sw.Stop();
            Console.WriteLine($"Elapsed; {sw.ElapsedMilliseconds}ms");
            Copy(riskToReach[end]); //Copies puzzle answer to clipboard (fortunately correct)

            int GetCostAt(Point point)
            { //gets the cost of a point outside of the original 100x100 grid for part 2
                int baseCost = input[point.X % input.GetLength(0), point.Y % input.GetLength(1)];
                baseCost += (point.X / input.GetLength(0)) + (point.Y / input.GetLength(1));
                return ((baseCost - 1) % 9) + 1;
            }
        }

        public override void Solve() => PartTwo();
    }
}
