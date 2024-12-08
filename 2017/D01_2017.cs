namespace AOC.Y2017;

internal class D01_2017 : Day
{
    public override void PartA()
    {
        Submit(InputLine.Select((x, i) => x == ValueAhead(i) ? (x - '0') : 0).Sum());
    }
    public override void PartB()
    {
        Submit(InputLine.Select((x, i) => x == ValueAhead(i, InputLine.Length / 2) ? (x - '0') : 0).Sum());
    }
    char ValueAhead(int index, int stepsAhead = 1) => InputLine[(index + stepsAhead) % InputLine.Length];
}
