using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2018;

internal class D01_2018 : Day<int>
{
    public override void PartOne()
    {
        Submit(Input.Sum());
    }
    public override void PartTwo()
    {
        Submit(Input.LoopForever().AggregateSteps((a, b) => a + b).Duplicates().First());
    }
}
