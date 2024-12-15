using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D06_2022 : Day
    {
        int PacketDetector(int length)
        {
            for (int i = 0; i <= InputLine.Length - length; i++)
            {
                if (InputLine.Skip(i).Take(length).Distinct().Count() == length) return i + length;
            }
            return -1;
        }
        public override void PartOne()
        {
            Submit(PacketDetector(4));
        }
        public override void PartTwo()
        {
            Submit(PacketDetector(14));
        }
    }
}
