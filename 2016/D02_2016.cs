using AOC.Items.Geometry;

namespace AOC.Y2016;

internal class D02_2016 : Day
{
    public override void PartOne()
    {
        int[,] keypad = new int[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        };
        Grid<int> grid = new(keypad.Transpose());

        Point2D nextPos, point = (1, 1);
        string bathroomCode = "";
        foreach (string line in Input)
        {
            foreach (char c in line)
            {
                Direction direction = c.ToDirection();
                nextPos = point.NextIn(direction);
                if (grid.PointInBounds(nextPos)) point = nextPos;
            }
            bathroomCode += grid[point].ToString();
        }
        Submit(bathroomCode);
    }
    public override void PartTwo()
    {
        char[,] keypad = new char[,]
        {
            { '0', '0', '1', '0', '0' },
            { '0', '2', '3', '4', '0' },
            { '5', '6', '7', '8', '9' },
            { '0', 'A', 'B', 'C', '0' },
            { '0', '0', 'D', '0', '0' }
        };
        Grid<char> grid = new(keypad.Transpose());

        Point2D nextPos, point = (1, 1);
        string bathroomCode = "";
        foreach (string line in Input)
        {
            foreach (char c in line)
            {
                Direction direction = c.ToDirection();
                nextPos = point.NextIn(direction);
                if (grid.PointInBounds(nextPos) && grid[nextPos] != '0') point = nextPos;
            }
            bathroomCode += grid[point].ToString();
        }
        Submit(bathroomCode);
    }
}
