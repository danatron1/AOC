using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D1_2021 : Day
    {
        public override void Solve() => PartB();
        public override void PartA() => IncreasesInDepth(1);
        public override void PartB() => IncreasesInDepth(3);
        void IncreasesInDepth(int windowSize = 1)
        {
            int[] input = GetInputForDay<int>(); //get input
            int increases = input.Skip(windowSize).Where((d, i) => d > input[i]).Count(); //solve puzzle
            Copy(increases); //copy result to clipboard
        }
    }
}
