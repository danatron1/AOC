using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D06_2023 : Day
{
    public override void PartA()
    {
        long[] times = Input[0].ExtractNumbers<long>();
        long[] distances = Input[1].ExtractNumbers<long>();
        Submit(GetWaysToWin(times, distances).Mul());
    }
    public override void PartB()
    {
        long[] times = Input[0].Replace(" ", null).ExtractNumbers<long>();
        long[] distances = Input[1].Replace(" ", null).ExtractNumbers<long>();
        Submit(GetWaysToWin(times, distances).MulAsLong());
    }
    int[] GetWaysToWin(long[] times, long[] distances)
    {
        int[] result = new int[times.Length];
        for (int i = 0; i < times.Length; i++)
        {
            int counter = 0;
            long hold = times[i] / 2;
            while (hold > 0)
            {
                if (hold * (times[i] - hold) <= distances[i]) break;
                counter += 2;
                hold--;
            }
            if (times[i] % 2 == 0) counter--;
            result[i] = counter;
        }
        return result;
    }
}
