using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items
{
    public class GridPathfinder<T> where T : notnull
    {
        public Grid<T> Grid;
        public Point2D? Target;
        public delegate bool ConnectionRule(T from, T to, Direction dir);
        public ConnectionRule ConnectivityTest = (_,_,_) => true;
        public delegate bool VictoryRule(Point2D on);
        public VictoryRule VictoryTest;
        public GridPathfinder(Grid<T> grid)
        {
            Grid = grid;
        }
        public GridPathfinder(Grid<T> grid, Point2D target) : this(grid)
        {
            Target = target;
            VictoryTest = p => p == target;
        }
        public GridPathfinder(Grid<T> grid, Point2D target, ConnectionRule connectionRule) : this(grid, target)
        {
            ConnectivityTest = connectionRule;
        }
        public GridPathfinder(Grid<T> grid, VictoryRule victoryRule) : this(grid)
        {
            VictoryTest = victoryRule;
        }
        public GridPathfinder(Grid<T> grid, VictoryRule victoryRule, ConnectionRule connectionRule) : this(grid, victoryRule)
        {
            ConnectivityTest = connectionRule;
        }

        public bool Reach(Point2D start, out PathfinderPoint2D found)
        {
            return Reach(new PathfinderPoint2D(start), out found);
        }
        public bool Reach(PathfinderPoint2D start, out PathfinderPoint2D found)
        {
            start.SetDistance(Target);
            List<PathfinderPoint2D> active = new();
            //PriorityQueue<PathfinderPoint2D, int> queue = new();
            HashSet<Point2D> visited = new();
            active.Add(start);
            do
            {
                found = active.OrderBy(x => x.CostDistance).First();
                if (VictoryTest(found.Point)) return true;
                active.Remove(found);
                visited.Add(found.Point);
                foreach (PathfinderPoint2D adjacent in GetWalkable(found))
                {
                    if (visited.Contains(adjacent.Point)) continue;
                    PathfinderPoint2D? existing = active.FirstOrDefault(x => x.Point == adjacent.Point);
                    if (existing is null) active.Add(adjacent);
                    else if (existing.CostDistance >= adjacent.CostDistance)
                    {
                        active.Remove(existing);
                        active.Add(adjacent);
                    }
                }
            } while (active.Any());

            return false;
        }
        private IEnumerable<PathfinderPoint2D> GetWalkable(PathfinderPoint2D from)
        {
            foreach (var adjacent in from.Point.AdjacentWithDirection())
            {
                if (!Grid.ContainsPoint(adjacent.point)) continue;
                if (!ConnectivityTest(Grid[from.Point], Grid[adjacent.point], adjacent.dir)) continue;
                PathfinderPoint2D adjacentTile = new PathfinderPoint2D(adjacent.point, from);
                adjacentTile.SetDistance(Target);
                yield return adjacentTile;
            }
        }
        public IEnumerable<PathfinderPoint2D> FloodFill(Point2D start) => FloodFill(new PathfinderPoint2D(start));
        public IEnumerable<PathfinderPoint2D> FloodFill(PathfinderPoint2D start)
        {
            List<PathfinderPoint2D> mappedArea = new();
            List<PathfinderPoint2D> active = new();
            HashSet<Point2D> visited = new();
            active.Add(start);
            PathfinderPoint2D next;
            do
            {
                next = active.OrderBy(x => x.CostDistance).First();
                active.Remove(next);
                mappedArea.Add(next);
                visited.Add(next.Point);
                foreach (PathfinderPoint2D adjacent in GetWalkable(next))
                {
                    if (visited.Contains(adjacent.Point)) continue;
                    PathfinderPoint2D? existing = active.FirstOrDefault(x => x.Point == adjacent.Point);
                    if (existing is null) active.Add(adjacent);
                    else if (existing.CostDistance >= adjacent.CostDistance)
                    {
                        active.Remove(existing);
                        active.Add(adjacent);
                    }
                }
            } while (active.Any());

            return mappedArea;
        }
        public class PathfinderPoint2D
        {
            public Point2D Point;
            public int Cost = 0;
            public int Distance;
            public int CostDistance => Cost + Distance;
            public PathfinderPoint2D? previous;
            public void SetDistance(Point2D? target)
            {
                if (target is null) Distance = 0;
                else Distance = Point.ManhattanDistanceTo(target.Value);
            }
            private Direction? ReachedFromDirection()
            {
                if (previous is null) return null;
                foreach (var d in Point.AdjacentWithDirection())
                {
                    if (d.point == previous.Point) return d.dir;
                }
                return null;
            }
            public PathfinderPoint2D(Point2D point)
            {
                Point = point;
            }
            public PathfinderPoint2D(Point2D point, PathfinderPoint2D from) : this(point)
            {
                previous = from;
                Cost = from.Cost + 1;
            }
        }
    }
}
