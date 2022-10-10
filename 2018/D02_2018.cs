using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D02_2018 : Day
    {
        public override void PartA()
        {
            string[] input = GetInputForDay();
            int twoOfs = 0;
            int threeOfs = 0;

            foreach (var item in input)
            {
                if (item.Any(c => item.FrequencyOf(c) == 2)) twoOfs++;
                if (item.Any(c => item.FrequencyOf(c) == 3)) threeOfs++;
            }
            Submit(twoOfs * threeOfs);
        }
        public override void PartB()
        {
            string[] input = GetInputForDay();

            foreach (string[] pair in input.Pairs())
            {
                string overlap = "";
                for (int c = 0; c < pair[0].Length; c++)
                {
                    if (pair[0][c] == pair[1][c]) overlap += pair[0][c];
                }

                if (overlap.Length == pair[0].Length - 1)
                {
                    Console.WriteLine($"{overlap} - {overlap.Length}/{pair[0].Length}");
                    Submit(overlap);
                    return;
                }
            }

            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < input.Length; y++)
                {
                    if (x == y) continue;
                    string overlap = "";
                    for (int c = 0; c < input[x].Length; c++)
                    {
                        if (input[x][c] == input[y][c]) overlap += input[x][c];
                    }
                    
                    if (overlap.Length == input[x].Length-1)
                    {
                        Console.WriteLine($"{overlap} - {overlap.Length}/{input[x].Length}");
                        Submit(overlap);
                        return;
                    }
                }
            }
        }
    }
}
