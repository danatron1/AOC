using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D19_2023 : Day
{
    struct Condition
    {
        private const string parameters = "xmas";
        public int Index;
        public bool Greater;
        public int Threshold;
        public string Next;

        public Condition(string line)
        {
            Next = line[(line.IndexOf(':')+1)..];
            Index = parameters.IndexOf(line[0]);
            Greater = line[1] == '>';
            Threshold = line.ExtractNumber<int>();
        }
        public readonly bool Test(int[] shape)
        {
            if (shape[Index] == 0) return true;
            if (Greater) return shape[Index] > Threshold;
            else return shape[Index] < Threshold;
        }
        internal PartRange ChunkFrom(PartRange range)
        {
            PartRange nextRange = new(Next);
            for (int i = 0; i < 4; i++)
            {
                nextRange.mins[i] = range.mins[i];
                nextRange.maxs[i] = range.maxs[i];
            }
            if (Greater)
            {
                nextRange.mins[Index] = Threshold + 1;
                range.maxs[Index] = Threshold;
            }
            else
            {
                nextRange.maxs[Index] = Threshold - 1;
                range.mins[Index] = Threshold;
            }
            return nextRange;
        }
    }
    class Workflow
    {
        readonly string Default;
        readonly List<Condition> Conditions = new();
        public Workflow(string line)
        {
            string[] split = line.Split(',');
            for (int i = 0; i < split.Length - 1; i++) Conditions.Add(new Condition(split[i]));
            Default = split[^1];
        }
        public string Process(int[] shape)
        {
            foreach (var condition in Conditions) if (condition.Test(shape)) return condition.Next;
            return Default;
        }

        internal IEnumerable<PartRange> ProcessRange(PartRange range)
        {
            foreach (var condition in Conditions) yield return condition.ChunkFrom(range);
            range.workflow = Default;
            yield return range;
        }
    }
    public override void PartA()
    {
         useExampleInput = true;
        Dictionary<string, Workflow> workflows = new();

        foreach (string line in InputBlocks[0]) 
        { //create workflows
            string[] split = line.Split('{');
            workflows[split[0]] = new Workflow(split[1].Trim('}'));
        }
        int sum = 0;
        foreach (string line in InputBlocks[1])
        { 
            int[] values = line.ExtractNumbers<int>();
            string printMessage = $"{values.ToContentString()}: in "; //purely cosmetic
            string workflow = "in";
            while (workflow.Length > 1)
            {
                workflow = workflows[workflow].Process(values);
                printMessage += $" -> {workflow}"; //cosmetic
            }
            if (workflow == "A") sum += values.Sum();
            Console.WriteLine(printMessage); //cosmetic
        }
        Submit(sum);
    }
    class PartRange
    {
        const int absoluteMin = 1;
        const int absoluteMax = 4000;
        public int[] mins = Enumerable.Repeat(absoluteMin, 4).ToArray();
        public int[] maxs = Enumerable.Repeat(absoluteMax, 4).ToArray();
        public string workflow;
        public PartRange(string wf)
        {
            workflow = wf;
        }
        public long Combinations() => mins.Zip(maxs).Select(x => 1 + x.Second - x.First).MulAsLong();
        public bool Exists() => mins.Zip(maxs).All(x => x.First <= x.Second);
    }
    public override void PartB()
    {
        //useExampleInput = true;
        Dictionary<string, Workflow> workflows = new();

        foreach (string line in InputBlocks[0])
        { //create workflows
            string[] split = line.Split('{');
            workflows[split[0]] = new Workflow(split[1].Trim('}'));
        }
        List<PartRange> partRanges = new() { new("in") };
        long maxPossible = partRanges[0].Combinations();
        int gen = 0;

        long rejectedTotal = 0;
        long total = 0;
        Console.WriteLine($"Gen {gen}: Sorted A: {total}, Sorted R: {rejectedTotal}, Processing: {maxPossible - rejectedTotal - total} ({partRanges.Count} ranges)");
        while (rejectedTotal + total < maxPossible)
        {
            List<PartRange> nextGen = new();
            foreach (PartRange range in partRanges)
            {
                if (!range.Exists()) continue;
                if (range.workflow == "R")
                {
                    rejectedTotal += range.Combinations();
                    continue;
                }
                if (range.workflow == "A")
                {
                    total += range.Combinations();
                    continue;
                }
                nextGen.AddRange(workflows[range.workflow].ProcessRange(range));
            }
            partRanges = nextGen;
            Console.WriteLine($"Gen {++gen}: Sorted A: {total}, Sorted R: {rejectedTotal}, Processing: {maxPossible - rejectedTotal - total} ({partRanges.Count} ranges)");
        }
        Submit(total);
    }
}
