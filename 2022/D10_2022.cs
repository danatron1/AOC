using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D10_2022 : Day
    {
        public override void PartOne()
        {
            int clock = 0, sum = 0, X = 1;
            foreach (string row in Input)
            {
                ClockPulse();
                if (!row.TryExtractNumber(out int value)) continue;
                ClockPulse();
                X += value;
            }
            void ClockPulse()
            {
                if (++clock % 40 == 20) sum += clock * X;
            }
            Submit(sum);
        }
        public override void PartTwo()
        {
            int clock = 0, X = 1;
            string line = "";
            foreach (string row in Input)
            {
                ClockPulse();
                if (!row.TryExtractNumber(out int value)) continue;
                ClockPulse();
                X += value;
            }
            //no submit - VISUAL
            void ClockPulse()
            {
                line += X.WithinRangeOf(1, clock % 40) ? '#' : '.';
                if (++clock % 40 == 0)
                {
                    Console.WriteLine(line);
                    line = "";
                }
            }
        }
    }
}
