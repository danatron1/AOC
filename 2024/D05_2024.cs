namespace AOC.Y2024;

internal class D05_2024 : Day
{
    public override void PartA()
    {
        FunkySorter sorter = new(InputBlocks[0]);
        int sum = 0;
        foreach (string pageList in InputBlocks[1])
        {
            int[] pages = pageList.ExtractNumbers<int>();
            if (pages.OrderBy(p => p, sorter).SequenceEqual(pages)) sum += pages[(pages.Length - 1) / 2];
        }
        Submit(sum);
    }
    public override void PartB()
    {
        FunkySorter sorter = new(InputBlocks[0]);
        int sum = 0;
        foreach (string pageList in InputBlocks[1])
        {
            int[] pages = pageList.ExtractNumbers<int>();
            int[] sorted = pages.OrderBy(p => p, sorter).ToArray();
            if (!sorted.SequenceEqual(pages)) sum += sorted[(sorted.Length - 1) / 2];
        }
        Submit(sum);
    }

    class FunkySorter : IComparer<int?>
    {
        private readonly HashSet<(int? x, int? y)> ComparisonRules;
        public FunkySorter(string[] rules)
        {
            ComparisonRules = new();
            foreach (string rule in rules)
            {
                int[] nums = rule.ExtractNumbers<int>();
                ComparisonRules.Add((nums[0], nums[1]));
            }
        }
        public int Compare(int? x, int? y)
        {
            if (x is null || y is null) return 0;
            if (ComparisonRules.Contains((x, y))) return -1;
            if (ComparisonRules.Contains((y, x))) return 1;
            return 0;
        }
    }
}
