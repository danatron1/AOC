using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D10_2023 : Day<char>
{
    Dictionary<char, string> pipeCharacters = new()
    {
        {'|', "│"},
        {'-', "─"},
        {'F', "┌"},
        {'7', "┐"},
        {'L', "└"},
        {'J', "┘"}
    };
    public override void PartOne()
    {
        //useExampleInput = true;
        Grid<char> grid = new Grid<char>(Input2D, '.');
        grid.printedRepresentation = pipeCharacters;
        Point2D start = grid.FindPoint('S')!.Value;
        grid[start] = FindImpliedSCharacter(grid, start);

        var pathfinder = grid.CreatePathfinder();
        pathfinder.TileConnectivityTest = PipeConnects;
        var nodes = pathfinder.FloodFill(start);
        Submit(nodes.MaxBy(x => x.Cost)!.Cost);
    }
    string hasNorthConnection = "|LJ";
    string hasEastConnection = "-LF";
    string hasSouthConnection = "|7F";
    string hasWestConnection = "-J7";
    private bool PipeConnects(char from, char to, Direction dir)
    {
        return dir switch
        {
            Direction.North => hasNorthConnection.Contains(from),
            Direction.East => hasEastConnection.Contains(from),
            Direction.South => hasSouthConnection.Contains(from),
            Direction.West => hasWestConnection.Contains(from),
            _ => false
        };
    }
    char FindImpliedSCharacter(Grid<char> grid, Point2D s)
    {
        bool north = hasSouthConnection.Contains(grid[s.North]),
            south = hasNorthConnection.Contains(grid[s.South]),
            east = hasWestConnection.Contains(grid[s.East]),
            west = hasEastConnection.Contains(grid[s.West]);
        if (north)
        {
            if (south) return '|';
            if (east) return 'L';
            if (west) return 'J';
        }
        if (east)
        {
            if (west) return '-';
            if (south) return 'F';
        }
        if (south && west) return '7';
        return 'S';
    }
    public override void PartTwo()
    {
        Grid<char> grid = new Grid<char>(Input2D, '.');
        grid.printedRepresentation = pipeCharacters;
        Point2D start = grid.FindPoint('S')!.Value;
        grid[start] = FindImpliedSCharacter(grid, start);

        var pathfinder = grid.CreatePathfinder();
        pathfinder.TileConnectivityTest = PipeConnects;
        var nodes = pathfinder.FloodFill(start);
        HashSet<Point2D> pipePoints = nodes.Select(x => x.Point).ToHashSet();
        int enclosed = 0;
        bool insidePipe = false;
        foreach (Point2D point in grid.Scan()) //left to right, top to bottom
        {
            if (pipePoints.Contains(point))
            {
                if (hasSouthConnection.Contains(grid[point])) insidePipe = !insidePipe;
            }
            else if (insidePipe)
            {
                enclosed++;
                grid[point] = '#';
            }
            else grid[point] = '.';
        }
        Submit(enclosed);
        grid.PrintBoard();
    }
}
