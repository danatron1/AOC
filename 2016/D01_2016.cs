using AOC.Items.Geometry;

namespace AOC.Y2016;

internal class D01_2016 : Day
{
    List<(char direction, int distance)> Steps = new();
    internal override void Setup()
    {
        string[] inputSplit = InputLine.Split(", ");
        foreach (var input in inputSplit)
        {
            Steps.Add((input[0], int.Parse(input[1..])));
        }
    }
    public override void PartOne()
    {
        //start location
        Direction direction = Direction.North;
        Point2D point = new(0, 0);

        foreach (var step in Steps)
        {
            direction = step.direction switch
            {
                'L' => direction.Left(),
                'R' => direction.Right(),
                _ => throw new NotImplementedException($"Direction character {step.direction} not handled")
            };
            point = point.NextIn(direction, step.distance);
        }
        Submit(point.ManhattanDistanceTo((0, 0)));
    }
    public override void PartTwo()
    {
        //start location
        Direction direction = Direction.North;
        Point2D point = new(0, 0);
        Grid<bool> grid = new();
        grid[point] = true;

        foreach (var step in Steps)
        {
            direction = step.direction switch
            {
                'L' => direction.Left(),
                'R' => direction.Right(),
                _ => throw new NotImplementedException($"Direction character {step.direction} not handled")
            };
            for (int i = 0; i < step.distance; i++)
            {
                point = point.NextIn(direction);
                if (grid[point])
                {
                    Submit(point.ManhattanDistanceTo((0, 0)));
                    return;
                }
                grid[point] = true;
            }
        }
    }
}
