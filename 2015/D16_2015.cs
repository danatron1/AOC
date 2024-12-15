using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC;

internal class D16_2015 : Day
{
    public override void PartOne()
    {
        Aunt[] aunts = Input.Select(row => new Aunt(row)).ToArray();
        Submit(aunts.First(a => a.properties.All(p => a.PropMatch(p))).ID);
    }
    public override void PartTwo()
    {
        Aunt[] aunts = Input.Select(row => new Aunt(row)).ToArray();
        Submit(aunts.First(a => a.properties.All(p => a.PropMatch(p, false))).ID);
    }
    class Aunt
    {
        const string name = "Sue";
        public static readonly Aunt auntToMatch = new($"Sue 0: children: 3, cats: 7, samoyeds: 2, pomeranians: 3, akitas: 0, vizslas: 0, goldfish: 5, trees: 3, cars: 2, perfumes: 1");
        public int ID;
        public Dictionary<string, int> properties = new();
        public Aunt(string line)
        {
            string[] parts = line.Split(' ').Select(s => s.Trim(',',':',' ')).ToArray();
            ID = int.Parse(parts[1]);
            for (int i = 2; i < parts.Length -1; i+=2)
            {
                properties.Add(parts[i], int.Parse(parts[i + 1]));
            }
        }
        public override string ToString()
        {
            return $"{name} {ID}: {string.Join(", ", properties.Select(k => $"{k.Key}: {k.Value}"))}";
        }
        static string[] lowBound = { "cats", "trees" };
        static string[] highBound = { "pomeranians", "goldfish" };
        public bool PropMatch(KeyValuePair<string, int> prop, bool retroencabulatorUpToDate = true)
        {
            if (!retroencabulatorUpToDate)
            {
                if (lowBound.Contains(prop.Key)) return prop.Value > auntToMatch.properties[prop.Key];
                if (highBound.Contains(prop.Key)) return prop.Value < auntToMatch.properties[prop.Key];
            }
            return prop.Value == auntToMatch.properties[prop.Key];
        }
    }
}
