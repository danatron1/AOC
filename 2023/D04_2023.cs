using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D04_2023 : Day
{
    public override void PartA()
    {
        Submit(Input.Select(x => x.ExtractNumbers<int>()).Select(x => 1<<x[1..11].Intersect(x[11..]).Count()>>1).Sum());
    }
    public override void PartB()
    {
        int[] wins = Input.Select(x => x.ExtractNumbers<int>()).Select(x => x[1..11].Intersect(x[11..]).Count()).ToArray();
        for (int i = wins.Length-1; i >= 0; i--) wins[i] += wins[(1+i)..(1+i+wins[i])].Sum();
        Submit(wins.Length + wins.Sum());
    }
}
