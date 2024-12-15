using AOC.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D04_2022 : Day
    {
        int[][]? splitInputs = null;
        public override void PartOne()
        {
            splitInputs ??= Input.Select(s => s.Split('-', ',').ConvertTo<int>().ToArray()).ToArray();
            Submit(splitInputs.Count(x => new Interval(x[0], x[1]).ContainsOrIsContainedBy(new Interval(x[2], x[3]))));
        }
        public override void PartTwo()
        {
            splitInputs ??= Input.Select(s => s.Split('-', ',').ConvertTo<int>().ToArray()).ToArray();
            Submit(splitInputs.Count(x => new Interval(x[0], x[1]).Overlap(new Interval(x[2], x[3])).HasArea));
        }
    }
}
