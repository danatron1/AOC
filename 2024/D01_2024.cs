using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D01_2024 : Day
{
    (int[] left, int[] right) GetLocationIDs(bool sorted)
    {
        (int[] left, int[] right) output;
        List<int[]> inputs = Input.Select(x => x.ExtractNumbers<int>()).ToList();
        output.left = inputs.Select(x => x[0]).ToArray();
        output.right = inputs.Select(x => x[1]).ToArray();
        if (sorted)
        {
            output.left.Sort();
            output.right.Sort();
        }
        return output;
    }
    public override void PartA()
    {
        (int[] left, int[] right) = GetLocationIDs(true);
        Submit(left.Select((l, i) => Math.Abs(l - right[i])).Sum());
    }
    public override void PartB()
    {
        (int[] left, int[] right) = GetLocationIDs(false);
        Tally<int> rightCounts = right.ToTally();
        Submit(left.Sum(x => x * rightCounts[x]));
    }
}
