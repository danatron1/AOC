namespace AOC.Y2016;

internal class D06_2016 : Day
{
    public override void PartOne()
    {
        Tally<char>[] tallies = GetTallies();
        Submit(new string(tallies.Select(x => x.First()).ToArray()));
    }
    public override void PartTwo()
    {
        Tally<char>[] tallies = GetTallies();
        Submit(new string(tallies.Select(x => x.Last()).ToArray()));
    }
    Tally<char>[] GetTallies()
    {
        Tally<char>[] tallies = new Tally<char>[InputLine.Length];
        for (int i = 0; i < tallies.Length; i++) tallies[i] = [];
        foreach (string line in Input)
        {
            for (int i = 0; i < line.Length; i++)
            {
                tallies[i].Add(line[i]);
            }
        }
        return tallies;
    }
}
