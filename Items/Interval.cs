using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items
{
    public struct Interval
    {
        public int Start;
        public int End;
        public int Size => 1 + End - Start;
        public bool HasArea => Size > 0;
        public Interval(int start, int end)
        {
            Start = start;
            End = end;
        }
        public static Interval FromRange(int a, int b) => new Interval(a, b - 1);
        public static Interval FromRangeInclusive(int a, int b) => new Interval(a, b);
        public Interval Overlap(Interval other) => new Interval(Math.Max(Start, other.Start), Math.Min(End, other.End));
        public bool Contains(Interval other) => other.End <= End && other.Start >= Start;
        public bool ContainsOrIsContainedBy(Interval other) => Contains(other) || other.Contains(this);
        public override string ToString() => $"{Start}--{End}";
    }
}
