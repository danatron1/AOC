using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC
{
    internal class D05_2015 : Day
    {
        public override void PartOne()
        {
            string[] naughtyList = GetInputForDay();
            Submit(naughtyList.Count(x => IsNice(x)));
        }
        public override void PartTwo()
        {
            string[] naughtyList = GetInputForDay();
            Submit(naughtyList.Count(x => IsNicev2(x)));
        }
        bool IsNice(string toTest)
        {
            if (toTest.FrequencyOf("aeiou".ToCharArray()) < 3) return false;
            if (toTest.FrequencyOf("ab", "cd", "pq", "xy") > 0) return false;
            for (int i = 1; i < toTest.Length; i++)
            {
                if (toTest[i] == toTest[i - 1]) return true;
            }
            return false;
        }
        bool IsNicev2(string toTest)
        {
            return Regex.IsMatch(toTest, "(\\w\\w)\\S*\\1")
                && Regex.IsMatch(toTest, "(\\w)\\S\\1");
        }
    }
}
