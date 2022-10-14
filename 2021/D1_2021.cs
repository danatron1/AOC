using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D1_2021 : Day<int>
    {
        public override void PartA() => Submit(IncreasesInDepth(1));
        public override void PartB() => Submit(IncreasesInDepth(3));
        int IncreasesInDepth(int windowSize = 1)
        {
            return Input.Skip(windowSize).Where((d, i) => d > Input[i]).Count(); //solve puzzle
        }
    }
}
