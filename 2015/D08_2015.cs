using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC
{
    internal class D08_2015 : Day
    {
        public override void PartA()
        {
            string[] input = GetInputForDay();
            int total = input.Sum(x => x.Length);
            input = input.Select(x => Regex.Unescape(x)[1..^1]).ToArray();
            total -= input.Sum(x => x.Length);
            Submit(total);
        }
        public override void PartB()
        {
            string[] input = GetInputForDay();
            int total = input.Sum(x => x.Length);
            input = input.Select(x => $"\"{Regex.Escape(x).Replace("\"", "\\\"")}\"").ToArray();
            total = input.Sum(x => x.Length) - total;
            Submit(total);
        }
    }
}
