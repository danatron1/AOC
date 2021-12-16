using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D10_2021 : Day
    {
        public override void Solve() => PartB();
        public override void PartA()
        {
            string chunkOpen = "([{<";
            string chunkClose = ")]}>";
            int[] scores = { 3, 57, 1197, 25137};
            string[] input = GetInputForDay();
            int corruptedScore = 0;
            Stack<int> toClose;
            foreach (string item in input)
            {
                toClose = new Stack<int>();
                for (int i = 0; i < item.Length; i++)
                {
                    if (chunkOpen.Contains(item[i])) toClose.Push(i);
                    else if (chunkClose.Contains(item[i]))
                    {
                        char pair = item[toClose.Peek()];
                        if (chunkClose.IndexOf(item[i]) != chunkOpen.IndexOf(item[toClose.Peek()]))
                        {
                            corruptedScore += scores[chunkClose.IndexOf(item[i])];
                            Console.WriteLine($"{item.Substring(0,50)}..., Expecting {chunkClose[chunkOpen.IndexOf(item[toClose.Peek()])]} (position {toClose.Peek(),-2})" +
                                $", but found {item[i]} (position {i}) instead. +{scores[chunkClose.IndexOf(item[i])]} points!");
                            break;
                        }
                        else toClose.Pop();
                    }
                }
            }
            Console.WriteLine("Total score for the corrupted characters:");
            Copy(corruptedScore);
        }

        public override void PartB()
        {
            string chunkOpen = "([{<";
            string chunkClose = ")]}>";
            int[] scores = { 3, 57, 1197, 25137 };
            string[] input = GetInputForDay();
            int corruptedScore = 0;
            long incompleteScore;
            List<long> incompleteScores = new List<long>();
            Stack<int> toClose;
            foreach (string item in input)
            {
                toClose = new Stack<int>();
                for (int i = 0; i < item.Length; i++)
                {
                    if (chunkOpen.Contains(item[i])) toClose.Push(i);
                    else if (chunkClose.Contains(item[i]))
                    {
                        char pair = item[toClose.Peek()];
                        if (chunkClose.IndexOf(item[i]) != chunkOpen.IndexOf(item[toClose.Peek()]))
                        {
                            corruptedScore += scores[chunkClose.IndexOf(item[i])];
                            Console.WriteLine($"Expecting {chunkClose[chunkOpen.IndexOf(item[toClose.Peek()])]} (position {toClose.Peek(),-2})" +
                                $", but found {item[i]} (position {i}) instead. +{scores[chunkClose.IndexOf(item[i])]} points!");
                            toClose.Clear();
                            break;
                        }
                        else toClose.Pop();
                    }
                }
                incompleteScore = 0;
                if (toClose.Count > 0)
                {
                    while (toClose.Count > 0)
                    {
                        incompleteScore *= 5;
                        incompleteScore += 1 + chunkOpen.IndexOf(item[toClose.Pop()]);
                    }
                    Console.WriteLine($"Line incomplete! Incomplete score +{incompleteScore}");
                    incompleteScores.Add(incompleteScore);
                }
            }
            incompleteScores.Sort();
            Console.WriteLine("Total score for the corrupted characters:");
            Copy(corruptedScore);
            Console.WriteLine("Median score for the incomplete rows:");
            Copy(incompleteScores[incompleteScores.Count / 2]);
        }

    }
}
