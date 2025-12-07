using AOC.Items.Geometry;

namespace AOC.Y2025;

internal class D07_2025 : Day<char>
{
    public override void PartOne()
    {
        int splits = 0;
        Grid<char> grid = new(Input2D);
        FireBeamFrom(grid.First(x => x.Value == 'S').Key);
        Submit(splits);

        void FireBeamFrom(Point2D point)
        {
            grid[point] = '|';
            point = point.South;
            if (grid[point] == '.') FireBeamFrom(point);
            else if (grid[point] == '^')
            {
                FireBeamFrom(point.West);
                FireBeamFrom(point.East);
                splits++;
            }
        }
    }
    public override void PartTwo()
    {
        Grid<char> grid = new(Input2D);
        long[] beams = new long[grid.GetLimitsOrExtremes().maxX + 1];
        foreach (var point in grid)
        {
            if (point.Value == 'S') beams[point.Key.X] = 1;
            else if (point.Value == '^')
            {
                beams[point.Key.X - 1] += beams[point.Key.X];
                beams[point.Key.X + 1] += beams[point.Key.X];
                beams[point.Key.X] = 0;
            }
        }
        Submit(beams.Sum());
    }
}
