using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D01_2022 : Day<int>
    {
        public override void PartA()
        {
            Submit(InputBlocks.Select(x => x.Sum()).Max());
        }
        public override void PartB()
        {
            Submit(InputBlocks.Select(x => x.Sum()).OrderDescending().Take(3).Sum());
        }
    }
}
