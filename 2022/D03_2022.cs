using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D03_2022 : Day
    {
        public override void PartOne()
        {
            int sum = 0;
            foreach (var item in Input)
            {
                string a = item[..(item.Length / 2)];
                string b = item[(item.Length/ 2)..];
                string c = a.Overlap(b, false);
                Console.WriteLine(c.Length);
                if (char.IsLower(c[0]))
                {
                    sum += 1 + c[0] - 'a';
                }
                else sum += 27 + c[0] - 'A';

            }
            Submit(sum);
        }
        public override void PartTwo()
        {
            int sum = 0;
            for (int i = 0; i < Input.Length; i+=3)
            {
                string c = Input[i].Overlap(Input[i+1], false).Overlap(Input[i+2], false);
                Console.WriteLine(c.Length);
                if (char.IsLower(c[0]))
                {
                    sum += 1 + c[0] - 'a';
                }
                else sum += 27 + c[0] - 'A';
            }
            Submit(sum);
        }
    }
}
