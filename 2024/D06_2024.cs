using AOC.Items.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D06_2024 : Day<char>
{
    public override void PartA()
    {
        Grid<char> grid = new(Input2D);
        Point2D start = grid.FirstOrDefault(c => c.Value == '^').Key;
        Direction direction = Direction.North;
        HashSet<Point2D> visitedPoints = new() { start };
        while (true)
        {
            visitedPoints.Add(start);
            //grid[start] = 'X';
            var next = start.NextIn(direction);
            if (!grid.ContainsPoint(next)) break;
            if (grid[next] == '#') direction = direction.Right();
            else start = next;
        }
        Submit(visitedPoints.Count);
    }
    public override void PartB()
    {
        Walker.Grid = new(Input2D);
        Queue<Walker> walkers = new();
        walkers.Enqueue(new(Walker.Grid.FirstOrDefault(c => c.Value == '^').Key, Direction.North, new()));
        while (walkers.TryDequeue(out Walker walker))
        {
            foreach (Walker w in walker.Process()) walkers.Enqueue(w);
        }
        Submit(Walker.LoopingBoxes.Count);
    }
    class Walker
    {
        public static Grid<char> Grid;
        public static HashSet<Point2D> LoopingBoxes = new();
        private static HashSet<Point2D> TryingBoxPosition = new();

        Point2D Position;
        Direction Direction;
        HashSet<(Point2D, Direction)> Visited;
        
        Point2D? boxPosition;
        public Walker(Point2D position, Direction direction, HashSet<(Point2D, Direction)> visited)
        {
            Position = position;
            Direction = direction;
            Visited = visited;
        }
        public IEnumerable<Walker> Process()
        {
            if (boxPosition is not null && LoopingBoxes.Contains(boxPosition.Value)) yield break;
            if (Visited.Contains((Position, Direction))) // in a loop
            {
                LoopingBoxes.Add(boxPosition!.Value);
                yield break;
            }
            Visited.Add((Position, Direction));

            Point2D next = Position.NextIn(Direction);
            if (!Grid.ContainsPoint(next)) yield break;
            if (Grid[next] == '#' || boxPosition == next) Direction = Direction.Right();
            else
            {
                if (boxPosition is null && !TryingBoxPosition.Contains(next))
                {
                    Walker newWalker = new(Position, Direction.Right(), new(Visited))
                    {
                        boxPosition = next,
                        Visited = new(Visited)
                    };
                    TryingBoxPosition.Add(next);
                    yield return newWalker;
                }
                Position = next;
            }
            yield return this;
        }
    }
}
