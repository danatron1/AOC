using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items.Operations
{
    internal static class Operations
    {
        public delegate T MathsOperation2N<T>(T a, T b) where T : INumber<T>;
        public static T Mul<T>(this T a, T b) where T : INumber<T> => a * b;
        public static T Add<T>(this T a, T b) where T : INumber<T> => a + b;
        public static T Div<T>(this T a, T b) where T : INumber<T> => a / b;
        public static T Sub<T>(this T a, T b) where T : INumber<T> => a - b;
        public static T Mod<T>(this T a, T b) where T : INumber<T> => a % b;
        public static MathsOperation2N<T> MathsOpFromChar<T>(char c) where T : INumber<T>
        {
            return c switch
            {
                '*' => Mul<T>,
                '+' => Add<T>,
                '/' => Div<T>,
                '-' => Sub<T>,
                '%' => Mod<T>,
                _ => throw new ArgumentException($"char {c} was not recognised as a maths operation"),
            };
        }
    }
}
