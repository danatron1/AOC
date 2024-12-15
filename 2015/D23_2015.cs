using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D23_2015 : Day
{
    int a, b, nextInstruction;
    public override void PartOne()
    {
        nextInstruction = 0;
        while (nextInstruction >= 0 && nextInstruction < Input.Length)
        {
            nextInstruction = ExecuteNext();
        }
        Submit(b);
    }
    public override void PartTwo()
    {
        nextInstruction = 0;
        a = 1;
        b = 0;
        while (nextInstruction >= 0 && nextInstruction < Input.Length)
        {
            nextInstruction = ExecuteNext();
        }
        Submit(b);
    }
    int ExecuteNext()
    {
        string[] split = Input[nextInstruction].Split(" ");
        split[1] = split[1].Trim(',');
        if (split.Length == 3) split[2] = split[2].Trim('+');
        switch (split[0])
        {
            case "inc":
                if (split[1] == "a") a++;
                else b++;
                return nextInstruction + 1;
            case "tpl":
                if (split[1] == "a") a *= 3;
                else b *= 3;
                return nextInstruction + 1;
            case "hlf":
                if (split[1] == "a") a /= 2;
                else b /= 2;
                return nextInstruction + 1;
            case "jmp":
                return nextInstruction + int.Parse(split[1]);
            case "jio":
                if (split[1] == "a" && a == 1) return nextInstruction + int.Parse(split[2]);
                if (split[1] == "b" && b == 1) return nextInstruction + int.Parse(split[2]);
                return nextInstruction + 1;
            case "jie":
                if (split[1] == "a" && a % 2 == 0) return nextInstruction + int.Parse(split[2]);
                if (split[1] == "b" && b % 2 == 0) return nextInstruction + int.Parse(split[2]);
                return nextInstruction + 1;
        }
        throw new ArgumentException($"Instruction {Input[nextInstruction]} not recognised");
    }
}
