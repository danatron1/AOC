using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2018;

internal class D01_2018 : Day<int>
{
    public override void PartA()
    {
        Submit(Input.Sum());
    }
    public override void PartB()
    {
        Submit(Input.LoopForever().AggregateSteps((a, b) => a + b).Duplicates().First());
    }
}
