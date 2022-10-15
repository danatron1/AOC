using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D20_2015 : Day<int>
{
    public override void PartA()
    {
        int presents = 0;
        int houseNumber = 0;
        while (presents < InputLine)
        {
            houseNumber += 6;
            presents = houseNumber.Factors().Sum()*10;
        }
        Submit(houseNumber);
    }

    public override void PartB()
    {
        int presents = 0;
        int houseNumber = 0;
        while (presents < InputLine)
        {
            houseNumber += 6;
            presents = houseNumber.Factors().Where(i => houseNumber / i <= 50).Sum() * 11;
        }
        Submit(houseNumber);
    }
}
