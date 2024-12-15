using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D03_2024 : Day
{
    const string patternA = @"mul\(\d+,\d+\)";
    const string patternB = @"mul\(\d+,\d+\)|don't\(\)|do\(\)";
    public override void PartOne()
    {
        Submit(Regex.Matches(string.Join(' ',Input), patternA).Select(x => x.Value.ExtractNumbers<int>().Mul()).Sum());
    }
    public override void PartTwo()
    {
        int total = 0;
        bool flag = true;
        foreach (string match in Regex.Matches(string.Join(' ', Input), patternB).Select(x => x.Value))
        {
            switch (match)
            {
                case "don't()": flag = false; break;
                case "do()": flag = true; break;
                default:
                    if (flag) total += match.ExtractNumbers<int>().Mul(); 
                    break;
            }
        }
        Submit(total);
    }
}
