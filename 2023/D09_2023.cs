using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D09_2023 : Day
{
    class Sequence
    {
        int[] elements;
        Sequence differences;
        bool allZero;
        public int PredictNext()
        {
            if (allZero) return 0;
            return differences.PredictNext() + elements.Last();
        }
        public int PredictPrevious()
        {
            if (allZero) return 0;
            return elements.First() - differences.PredictPrevious();
        }
        public Sequence(int[] sequence)
        {
            elements = sequence.ToArray();
            allZero = elements.All(x => x == 0);
            if (!allZero) differences = new Sequence(elements.SelectWithPrevious((prev, curr) => curr - prev).ToArray());
        }
    }
    public override void PartOne()
    {
        Submit(Input.Select(x => new Sequence(x.ExtractNumbers<int>())).Sum(x => x.PredictNext()));
    }
    public override void PartTwo()
    {
        Submit(Input.Select(x => new Sequence(x.ExtractNumbers<int>())).Sum(x => x.PredictPrevious()));
    }
}
