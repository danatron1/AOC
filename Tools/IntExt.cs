using AOC.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class IntExt
{
    public static int[] Digits(this int i, int numberBase = 10)
    {
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
    public static int Choose(this IEnumerable<object> n, int k) => n.Count().Choose(k);
    public static int Choose(this int n, int k)
    {
        int r = 1;
        int d;
        if (k > n) return 0;
        for (d = 1; d <= k; d++)
        {
            r *= n--;
            r /= d;
        }
        return r;
    }
    public static int Factorial(this int i)
    {
        if (i < 3) return i;
        return i * Factorial(i - 1);
    }
    public static bool WithinRangeOf(this int x, int range, int value) => value - range <= x && x <= value + range;
    public static T GCF<T>(T a, T b) where T : INumber<T>
    {
        while (b != T.Zero)
        {
            T temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    public static T LCM<T>(T a, T b) where T : INumber<T>
    {
        return a / GCF(a, b) * b;
    }
    public static string BitString(this int x, int numBase = 2, bool padToByte = true, bool gapEveryByte = true, int minLength = 0)
    {
        int padding = 0;
        if (padToByte)
        {
            for (int i = x; i is not (0 or -1); i>>=8)
            {
                padding += 8;
            }
        }
        padding = Math.Max(padding, minLength);
        string bits = Convert.ToString(x, numBase).PadLeft(padding, '0');
        if (gapEveryByte)
        {
            for (int i = bits.Length; i > 8; i-=8)
            {
                int addSpace = ((i-1) >> 3) << 3;
                bits = bits[..addSpace] + ' ' + bits[addSpace..];
            }
        }
        return bits;
    }
}
