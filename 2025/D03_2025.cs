namespace AOC.Y2025;

internal class D03_2025 : Day
{
    public override void PartOne()
    {
        Submit(Input.Sum(x => MaxJoltage(x, 2)));
    }
    public override void PartTwo()
    {
        Submit(Input.Sum(x => MaxJoltage(x, 12)));
    }
    long MaxJoltage(string batteryBank, int batteries) => MaxJoltage(batteryBank.Select(x => x - '0').ToArray(), batteries);
    long MaxJoltage(IEnumerable<int> batteryBank, int batteries)
    {
        batteries--;
        int max = batteryBank.Take(batteryBank.Count() - batteries).Max();
        if (batteries == 0) return max;
        return (long)Math.Pow(10, batteries) * max + MaxJoltage(batteryBank.SkipWhile(x => x != max).Skip(1), batteries);
    }
}
