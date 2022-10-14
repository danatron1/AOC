using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D10_2015 : Day
    {
        public override void PartA()
        {
            List<Section> sections = IterateSequence(40, GetInputForDay()[0]);
            Submit(sections.Count * 2);
        }
        public override void PartB()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Section> sections = IterateSequence(50, GetInputForDay()[0]);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " milliseconds");
            Submit(sections.Count * 2);
        }
        List<Section> IterateSequence(int times, string start)
        {
            List<Section> sections = LookAndSay(start);
            for (int i = 1; i < times; i++)
            {
                sections = LookAndSay(sections);
            }
            return sections;
        }
        List<Section> LookAndSay(string last)
        {
            List<Section> sections = new List<Section>();
            Section current = new Section((byte)(last[0] - '0'));
            for (int i = 1; i < last.Length; i++)
            {
                if (last[i] - '0' == current.character) current.count++;
                else
                {
                    sections.Add(current);
                    current = new Section(byte.Parse(last[i].ToString()));
                }
            }
            sections.Add(current);
            return sections;
        }
        List<Section> LookAndSay(List<Section> last)
        {
            List<Section> next = new List<Section>();
            bool lastCountConsidered = false;
            for (int i = 0; i < last.Count; i++)
            {
                Section s = lastCountConsidered ? new(last[i].character) : new(last[i].count);
                if (s.character == last[i].character)
                {
                    if (!lastCountConsidered) s.count++;
                    lastCountConsidered = i + 1 < last.Count && s.character == last[i + 1].count;
                    if (lastCountConsidered) s.count++;
                }
                else
                {
                    next.Add(s);
                    s = new(last[i].character);
                    lastCountConsidered = i + 1 < last.Count && s.character == last[i + 1].count;
                    if (lastCountConsidered) s.count++;
                }
                next.Add(s);
            }
            return next;
        }
        struct Section
        {
            public byte character;
            public byte count;
            public Section(byte character, byte count = 1)
            {
                this.character = character;
                this.count = count;
            }
            public override string ToString()
            {
                return $"{count}{character}";
            }
        }
    }
}
