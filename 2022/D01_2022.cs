using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D01_2022 : Day<int>
    {
        public override void PartOne()
        {
            Submit(InputBlocks.Select(x => x.Sum()).Max());
        }
        public override void PartTwo()
        {
            Submit(InputBlocks.Select(x => x.Sum()).OrderDescending().Take(3).Sum());
        }
    }
}
