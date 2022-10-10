using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D02_2015 : Day
    {
        Present[] GetPresents()
        {
            string[] input = GetInputForDay();
            Present[] presents = new Present[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                presents[i] = new Present(input[i]);
            }
            return presents;
        }
        public override void PartA()
        {
            int total = GetPresents().Sum(x => x.paperNeeded);
            Submit(total);
        }
        public override void PartB()
        {
            int total = GetPresents().Sum(x => x.ribbonNeeded);
            Submit(total);
        }
        class Present
        {
            public int length, width, height;
            public int topSide => width * length; //always the smallest, due to ordering
            public int leftSide => length * height;
            public int frontSide => width * height;
            public int surfaceArea => (topSide + leftSide + frontSide) << 1;
            public int paperNeeded => surfaceArea + topSide;
            public int ribbonNeeded => ((length + width) << 1) + volume;
            public int volume => length * width * height;
            public Present(string dimensions)
            {
                int[] d = dimensions.Split('x').Select(x => int.Parse(x)).ToArray();
                Array.Sort(d);
                length = d[0];
                width = d[1];
                height = d[2];
            }
        }
    }
}
