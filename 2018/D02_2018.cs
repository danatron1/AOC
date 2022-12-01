using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D02_2018 : Day
    {
        public override void PartA()
        {
            int twoOfs = 0;
            int threeOfs = 0;
            foreach (var item in Input)
            {
                if (item.Any(c => item.FrequencyOf(c) == 2)) twoOfs++;
                if (item.Any(c => item.FrequencyOf(c) == 3)) threeOfs++;
            }
            Submit(twoOfs * threeOfs);
        }
        public override void PartB()
        {

            foreach (string[] pair in Input.Pairs())
            {
                if (pair[0].Overlap(pair[1]).Length == pair[0].Length - 1)
                {
                    Submit(pair[0].Overlap(pair[1]));
                    return;
                }
            }
        }
    }
}
