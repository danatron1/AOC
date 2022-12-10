using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D10_2022 : Day
    {
        public override void PartA()
        {
            int sum = 0;
            int clock = 0;
            int X = 1;
            foreach (var item in Input)
            {
                ClockPulse();
                string[] split = item.Split(' ');
                if (split[0] == "noop") continue;
                ClockPulse();
                int value = int.Parse(split[1]);
                X += value;
            }
            Submit(sum);
            void ClockPulse()
            {
                clock++;
                if (clock % 40 == 20 && clock < 240) sum += clock * X;
            }
        }
        public override void PartB()
        {
            int clock = 0;
            string line = "";
            int X = 1;
            foreach (var item in Input)
            {
                ClockPulse();
                string[] split = item.Split(' ');
                if (split[0] == "noop") continue;
                int value = int.Parse(split[1]);
                ClockPulse();
                X += value;
            }
            void ClockPulse()
            {
                if (Within1OfX(clock % 40))
                {
                    line += '#';
                }
                else line += '.';
                clock++;
                if (clock % 40 == 0)
                {
                    Console.WriteLine(line);
                    line = "";
                }
            }
            bool Within1OfX(int value)
            {
                if (value == X) return true;
                if (value + 1 == X) return true;
                if (value - 1 == X) return true;
                return false;
            }
        }
    }
}
