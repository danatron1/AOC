using AOC.Items.Geometry;

namespace AOC.Y2025;

internal class D04_2025 : Day<char>
{
    public override void PartOne()
    {
        Grid<char> grid = new(Input2D);
        Submit(GetAccessibleRolls(grid).Count());

    }
    public override void PartTwo()
    {
        Grid<char> grid = new(Input2D);
        IEnumerable<Point2D> rolls;
        do
        {
            rolls = GetAccessibleRolls(grid);
            foreach (Point2D roll in rolls)
            {
                grid[roll] = 'x';
            }
        } while (rolls.Any());
        Submit(grid.Count(x => x.Value == 'x'));
    }

    IEnumerable<Point2D> GetAccessibleRolls(Grid<char> grid)
    {
        return grid.Where(x => x.Value == '@' && x.Key.Surrounding().Count(x => grid[x] == '@') < 4).Select(x => x.Key);
    }
}
