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
    public static IEnumerable<int> Factors(this int number)
    {
        int max = (int)Math.Sqrt(number);
        for (int i = 1; i <= max; i++)
        {
            if (number % i == 0)
            {
                yield return i;
                if (i == max && i * i == number) break;
                yield return number / i;
            }
        }
    }
}
