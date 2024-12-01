using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D01_2019 : Day<int>
    {
        public override void PartA()
        {
            int sum = 0;
            foreach (int input in Input)
            {
                sum += (input / 3) - 2;
            }
            Copy(sum);
        }
        public override void PartB()
        {
            int sum = 0;
            foreach (int input in Input)
            {
                sum += GetFuelFor(input);
            }
            Copy(sum);
        }
        int GetFuelFor(int mass)
        {
            mass /= 3;
            mass -= 2;
            if (mass > 0) return mass + GetFuelFor(mass);
            return 0;
        }
    }
}
