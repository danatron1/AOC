using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D15_2015 : Day
    {
        public override void PartA()
        {
            int bestScore = 0;
            foreach (IEnumerable<int> spoonDistribution in 100.SplitToChunks(Input.Length).ToArray())
            {
                int[] spoons = spoonDistribution.ToArray();
                int[][] multipliers = Input.Select(row => row.PullColumns(2, 4, 6, 8)
                                                .Select(s => int.Parse(s.Trim(',')))
                                                .ToArray())
                                            .ToArray()
                                            .Transpose();
                int score = multipliers.Select(m => Math.Max(0, m.Select((p, i) => p * spoons[i])
                                                .Sum()))
                                            .Mul();
                if (score > bestScore) bestScore = score;
            }
            Submit(bestScore);
        }
        public override void PartB()
        {
            int bestScore = 0;
            foreach (IEnumerable<int> spoonDistribution in 100.SplitToChunks(Input.Length).ToArray())
            {
                int[] spoons = spoonDistribution.ToArray();
                int[] calories = Input
                    .Select(row => int.Parse(row.PullColumns(10).First()))
                    .ToArray();
                if (spoons.Select((s, i) => s * calories[i]).Sum() != 500) 
                    continue; //ensure 500 calories
                int[][] multipliers = Input
                    .Select(row => row.PullColumns(2, 4, 6, 8)
                        .Select(s => int.Parse(s.Trim(',')))
                        .ToArray())
                    .ToArray()
                    .Transpose();
                int score = multipliers
                    .Select(m => Math.Max(0, m
                        .Select((p, i) => p * spoons[i])
                        .Sum()))
                    .Mul();
                if (score > bestScore) bestScore = score;
            }
            Submit(bestScore);
        }
    }
}
