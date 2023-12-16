using AOC.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D14_2023 : Day<char>
{
    Grid<char> ShiftRocks(Grid<char> grid, Direction direction)
    {
        Grid<char> nextGrid = new Grid<char>();

        var limits = grid.GetLimitsOrExtremes();
        //assume cube rocks exist along the edges, too
        IEnumerable<Point2D> edge = direction switch
        {
            Direction.East => grid.PointsInColumn(limits.maxX + 1),
            Direction.South => grid.PointsInRow(limits.minY - 1),
            Direction.West => grid.PointsInColumn(limits.minX - 1),
            _ => grid.PointsInRow(limits.maxY + 1) //default to north
        };

        Point2D wall;
        foreach (Point2D topPoint in edge)
        {
            wall = topPoint;
            foreach (Point2D walk in grid.WalkToEdge(topPoint, direction.Opposite()))
            {
                switch (grid[walk])
                {
                    case '#':
                        nextGrid[walk] = '#';
                        wall = walk;
                        break;
                    case 'O':
                        wall = wall.NextIn(direction.Opposite());
                        nextGrid[wall] = 'O';
                        break;
                }
            }
        }
        return nextGrid;
    }
    int TotalLoad(Grid<char> grid, Direction direction)
    {
        IEnumerable<int> axis = direction.Vertical() ? grid.Rows() : grid.Columns();
        if (direction == Direction.North || direction == Direction.West) axis = axis.Reverse();
        int totalLoad = 0, rowLoad = 0;
        foreach (int line in axis)
        {
            rowLoad++;
            var values = direction.Vertical() ? grid.ValuesInRow(line) : grid.ValuesInColumn(line);
            totalLoad += rowLoad * values.Count(x => x.Value == 'O');
        }
        return totalLoad;
    }
    public override void PartA()
    {
        Grid<char> grid = new Grid<char>(Input2D, '.');
        grid = ShiftRocks(grid, Direction.North);
        Submit(TotalLoad(grid, Direction.North));
    }
    public override void PartB()
    {
        //useExampleInput = true;
        Grid<char> grid = new Grid<char>(Input2D, '.');
        Direction[] order = { Direction.North, Direction.West, Direction.South, Direction.East };
        HashSet<int> seen = new HashSet<int>();
        int cycleIdentifier = 0, cycleLength = 0, targetOffset = 0, run = 0;
        int maxCycles = 1_000_000_000;
        for (int i = 0; i < maxCycles; i++)
        {
            if (i % 10 == 0 && cycleIdentifier == 0) Console.WriteLine($"[{i}] Waiting for cycle...");
            foreach (Direction dir in order) grid = ShiftRocks(grid, dir);
            run = TotalLoad(grid, Direction.North) + 1000 * TotalLoad(grid, Direction.East);
            if (cycleIdentifier != 0) //found repeat
            {
                if (targetOffset != 0) //know cycle length
                {
                    Console.WriteLine($"[{i}] Waiting for offset {targetOffset}... ({i%cycleLength}/{targetOffset})");
                    if (i % cycleLength == targetOffset) break;
                }
                else
                {
                    cycleLength++;
                    Console.WriteLine($"[{i}] (cycle length {cycleLength}) This cycle: {run} (north load {TotalLoad(grid, Direction.North)})");
                    if (run == cycleIdentifier)
                    {
                        targetOffset = (maxCycles-1) % cycleLength;
                        Console.WriteLine($"[{i}] Target will be hit at offset {targetOffset} in a cycle of length {cycleLength}.");
                    }
                }
            }
            else if (seen.Contains(run))
            {
                Console.WriteLine($"[{i}] Cycle identified with ID {run}");
                cycleIdentifier = run;
            }
            else seen.Add(run);
        }
        Submit(TotalLoad(grid, Direction.North));
    }
}
