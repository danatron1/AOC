using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class IntExt
{
    public static int[] Digits(this int i, int numberBase = 10)
    {
        //789
        List<int> digits = new();
        while (i > 0)
        {
            digits.Add(i % numberBase);
            i /= numberBase;
        }
        digits.Reverse();
        return digits.ToArray();
    }
}
