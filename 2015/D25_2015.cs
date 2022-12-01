using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D25_2015 : Day
{
    public override void PartA()
    {
        int[] rowAndCol = InputLine.ExtractNumbers().Select(s => (int)s).ToArray();
        int index = Enumerable.Range(1, rowAndCol[0] - 1 + rowAndCol[1]).Sum() - rowAndCol[0];
        long startValue = 20151125;
        Submit(startValue.RepeatAction(NextValue, index));
    }
    public override void PartB()
    {
        Console.WriteLine("Started 06/10/2022");
        Console.WriteLine("Beaten on 18/10/2022. Only a little late :)");
    }
    long NextValue(long last) => last * 252533 % 33554393;
}
