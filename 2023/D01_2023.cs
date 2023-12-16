using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D01_2023 : Day
{

    public override void PartA()
    {
        int sum = 0;
        foreach (string line in Input)
        {
            sum += (line.First(char.IsNumber) - '0') * 10;
            sum += line.Last(char.IsNumber) - '0';
        }
        Submit(sum);
    }
    public override void PartB()
    {
        string[] numbers = new string[]
        {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };
        int sum = 0;
        foreach (string line in Input)
        {
            bool found = false;
            for (int i = 0; i < line.Length; i++) //scan from start
            {
                if (char.IsNumber(line[i]))
                {
                    sum += (line[i] - '0') * 10;
                    break;
                }
                foreach (string num in numbers)
                {
                    if (line[i..].StartsWith(num))
                    {
                        sum += (Array.IndexOf(numbers, num) + 1) * 10;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            found = false;
            for (int i = line.Length - 1; i >= 0; i--) //scan from end
            {
                if (char.IsNumber(line[i]))
                {
                    sum += line[i] - '0';
                    break;
                }
                foreach (string num in numbers)
                {
                    if (line[i..].StartsWith(num))
                    {
                        sum += Array.IndexOf(numbers, num) + 1;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
        }
        Submit(sum);
    }
}
