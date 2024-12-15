using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D24_2015 : Day<int>
{
    public override void PartOne()
    {
        return;

        //Works for short inputs like the example, not bigger ones!
        //useExampleInput = true;
        //Copy(Input.Combinations()
        //    .Where(a => a.Sum() == weightToMatch)
        //    .OrderBy(b => b.Count())
        //    .ThenBy(b => b.Mul()).First().Mul());

        int searched = 0, weightToMatch = Input.Sum() / 3;
        long smallestGroup = long.MaxValue, lowestQE = long.MaxValue;
        Console.WriteLine(weightToMatch);
        foreach (var item in Input.Combinations()
            .Where(a => a.Sum() == weightToMatch))
            //.OrderBy(b => b.Count()))
            //.ThenBy(b => b.Mul()))
        {
            searched++;
            if (item.Count() > smallestGroup) continue;
            if (item.Count() == smallestGroup && item.MulAsLong() >= lowestQE) continue;
            IEnumerable<int>? rest = Input.Except(item).Combinations()
                .Where(a => a.Sum() == weightToMatch).FirstOrDefault();
            if (rest == null) continue;
            smallestGroup = item.Count();
            lowestQE = item.MulAsLong();
            Console.WriteLine($"After {searched} searches, found new best with size {smallestGroup} and QE of {lowestQE} - {item.ToContentString()}");
        }
        Submit(lowestQE);
    }
    public override void PartTwo()
    {
        int searched = 0, weightToMatch = Input.Sum() / 4;
        long smallestGroup = long.MaxValue, lowestQE = long.MaxValue;
        Console.WriteLine(weightToMatch);
        foreach (var item in Input.Combinations()
            .Where(a => a.Sum() == weightToMatch))
        {
            searched++;
            if (item.Count() > smallestGroup) continue;
            if (item.Count() == smallestGroup && item.MulAsLong() >= lowestQE) continue;
            //IEnumerable<int>? rest = Input.Except(item).Combinations()
            //    .Where(a => a.Sum() == weightToMatch).FirstOrDefault();
            //if (rest == null) continue;
            smallestGroup = item.Count();
            lowestQE = item.MulAsLong();
            Console.WriteLine($"After {searched} searches, found new best with size {smallestGroup} and QE of {lowestQE} - {item.ToContentString()}");
        }
        Submit(lowestQE);
    }
}
