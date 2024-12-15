using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D15_2023 : Day
{
    int HASH(string input) => input.Aggregate(0, (a, b) => (a + b) * 17 % 256);
    public override void PartOne()
    {
        Submit(InputLine.Split(',').Sum(HASH));
    }
    public override void PartTwo()
    {
        OrderedDictionary[] boxes = new OrderedDictionary[256];
        for (int i = 0; i < boxes.Length; i++) boxes[i] = new OrderedDictionary();
        foreach (string instruction in InputLine.Split(","))
        {
            string label = string.Concat(instruction.TakeWhile(char.IsLetter));
            if (instruction[label.Length] == '-') boxes[HASH(label)].Remove(label);
            else boxes[HASH(label)][label] = instruction[^1] - '0';
        }
        int focusSum = 0;
        for (int b = 0; b < boxes.Length; b++)
        {
            for (int l = 0; l < boxes[b].Count; l++)
            {
                focusSum += (int)boxes[b][l]! * (b + 1) * (l + 1);
            }
        }
        Submit(focusSum);
    }
}
