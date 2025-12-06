namespace AOC.Y2025;

internal class D05_2025 : Day
{
    public override void PartOne()
    {
        HashSet<(long lower, long upper)> ranges = new();
        int i;
        for (i = 0; i < Input.Length; i++)
        {
            if (Input[i] == "") break;
            long[] rangeBoundaries = Input[i].ExtractNumbers<long>();
            ranges.Add((rangeBoundaries[0], rangeBoundaries[1]));
        }
        i++;
        int fresh = 0;
        for (; i < Input.Length; i++)
        {
            long number = long.Parse(Input[i]);
            if (ranges.Any(x => InRange(x, number))) fresh++;
        }
        Submit(fresh);
    }
    public override void PartTwo()
    {
        List<(long lower, long upper)> ranges = new();
        long fresh = 0;
        for (int i = 0; i < Input.Length; i++)
        {
            if (Input[i] == "") break;
            long[] rangeBoundaries = Input[i].ExtractNumbers<long>();
            (long lower, long upper) newRange = (rangeBoundaries[0], rangeBoundaries[1]);
            for (int x = 0; x < ranges.Count; x++)
            {
                if (InRange(newRange, ranges[x].lower) && InRange(newRange, ranges[x].upper))
                {
                    ranges.RemoveAt(x);
                    x--;
                    continue;
                }
                if (InRange(ranges[x], newRange.lower)) newRange.lower = ranges[x].upper + 1;
                if (InRange(ranges[x], newRange.upper)) newRange.upper = ranges[x].lower - 1;
                if (newRange.lower > newRange.upper) break;
            }
            if (newRange.lower <= newRange.upper) ranges.Add(newRange);
        }
        fresh = ranges.Sum(x => 1+ x.upper - x.lower);
        Submit(fresh);
    }
    bool InRange((long lower, long upper) range, long number) => number >= range.lower && number <= range.upper;
}
