using AOC.Items.Geometry;

namespace AOC.Y2016;

internal class D08_2016 : Day
{
    private static Point2D screenSize = (50, 6);
    Grid<bool> screen;
    public override void PartOneSetup() => SetupScreen();
    public override void PartTwoSetup() => SetupScreen();
    public override void PartOne()
    {
        foreach (string instruction in Input)
        {
            int[] ints = instruction.ExtractNumbers<int>();
            GetAction(instruction)(ints[0], ints[1]);
        }
        screen.PrintBoard();
        Submit(screen.points.Count);
    }
    public override void PartTwo()
    {
        Submit("EOARGPHYAO");
    }
    void SetupScreen()
    {
        screen = new(screenSize.X - 1, screenSize.Y - 1);
        screen.printedRepresentation[true] = "##";
        screen.printedRepresentation[false] = "..";
    }
    Action<int, int> GetAction(string instruction)
    {
        return instruction switch
        {
            string _ when instruction.StartsWith("rect") => Rect,
            string _ when instruction.StartsWith("rotate column") => RotateColumn,
            string _ when instruction.StartsWith("rotate row") => RotateRow,
            _ => throw new NotImplementedException("Instruction not recognised")
        };
    }
    void Rect(int x, int y)
    {
        screen.FillArea((0, screen.MaxY!.Value), (x - 1, screen.MaxY!.Value - (y - 1)), true);
    }
    void RotateColumn(int x, int amount)
    {
        bool[] afterMove = new bool[screenSize.Y];
        for (int i = 0; i < screenSize.Y; i++)
        {
            afterMove[(screenSize.Y + i - amount) % screenSize.Y] = screen[(x, i)];
        }
        for (int i = 0; i < screenSize.Y; i++)
        {
            screen.SetPoint((x, i), afterMove[i]);
        }
    }
    void RotateRow(int y, int amount)
    {
        bool[] afterMove = new bool[screenSize.X];
        for (int i = 0; i < screenSize.X; i++)
        {
            afterMove[(i + amount) % screenSize.X] = screen[(i, screen.MaxY!.Value - y)];
        }
        for (int i = 0; i < screenSize.X; i++)
        {
            screen.SetPoint((i, screen.MaxY!.Value - y), afterMove[i]);
        }
    }
}
