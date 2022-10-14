using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D14_2021 : Day
    {
        Tally<string> pairs;
        Tally<char> letterCounts;
        Dictionary<string, char> rules;
        public override void PartA()
        {
            LoadData();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IteratePolymer(10);
            sw.Stop();
            Console.WriteLine($"Calculated answer in {sw.ElapsedMilliseconds}ms");
            Copy(letterCounts.Max() - letterCounts.Min());
        }

        public override void PartB()
        {
            LoadData();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IteratePolymer(40);
            sw.Stop();
            Console.WriteLine($"Calculated answer in {sw.ElapsedMilliseconds}ms");
            Copy(letterCounts.Max() - letterCounts.Min());
        }
        void LoadData()
        {
            rules = new Dictionary<string, char>();
            letterCounts = new Tally<char>();
            pairs = new Tally<string>();
            string[] input = GetInputForDay();
            //load in rules
            for (int i = 2; i < input.Length; i++)
            {
                rules[input[i][..2]] = input[i][^1];
            }
            //load in starting input
            for (int i = 0; i < input[0].Length - 1; i++)
            {
                pairs.Add(input[0].Substring(i, 2));
            }
            letterCounts.AddRange(input[0].ToCharArray());
        }
        void IteratePolymer(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                Tally<string> newPairs = new Tally<string>();
                foreach (var rule in rules)
                {
                    newPairs.Add($"{rule.Key[0]}{rule.Value}", pairs[rule.Key]);
                    newPairs.Add($"{rule.Value}{rule.Key[1]}", pairs[rule.Key]);
                    letterCounts.Add(rule.Value, pairs[rule.Key]);
                }
                pairs = newPairs;
            }
        }
        public override void Solve() => PartB();
    }
}
