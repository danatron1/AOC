using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D24_2023 : Day
{
    Random rng = new Random();
    int a = 0;
    int x 
    { 
        get
        {
            if (rng.Next(10) == 1) return a + 1;
            else return a;
        }
    }
    public override void PartOne()
    {
        Console.WriteLine("\n\n\n");

        for (int i = 0; i < 12; i++)
        {
            Console.WriteLine(x == x);
        }
    }
    public override void PartTwo()
    {
        throw new NotImplementedException();
    }
}
