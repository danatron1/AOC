using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D05_2022 : Day
    {
        public override void PartA()
        {
            //initialize the starting positions
            Stack<char>[] stacks = new Stack<char>[(int)InputBlocks[0][^1].ExtractNumbers().Max()];
            for (int r = InputBlocks[0].Length - 2; r >= 0; r--)
            {
                for (int s = 0; s < stacks.Length; s++)
                {
                    char c = InputBlocks[0][r][4 * s + 1];
                    if (char.IsLetter(c))
                    {
                        stacks[s] ??= new Stack<char>();
                        stacks[s].Push(c);
                    }
                }
            }
            //execute the moves
            foreach (string line in InputBlocks[1])
            {
                int[] move = line.ExtractNumbers<int>();
                for (int i = 0; i < move[0]; i++)
                {
                    stacks[move[2] - 1].Push(stacks[move[1] - 1].Pop());
                }
            }
            //get final code
            string code = new string(stacks.Select(x => x.Peek()).ToArray());
            Submit(code);
        }
        public override void PartB()
        {
            //initialize the starting positions
            Stack<char>[] stacks = new Stack<char>[(int)InputBlocks[0][^1].ExtractNumbers().Max() + 1];
            stacks[0] = new Stack<char>();  
            for (int r = InputBlocks[0].Length - 2; r >= 0; r--)
            {
                for (int s = 1; s < stacks.Length; s++)
                {
                    char c = InputBlocks[0][r][4 * s -3];
                    if (char.IsLetter(c))
                    {
                        stacks[s] ??= new Stack<char>();
                        stacks[s].Push(c);
                    }
                }
            }
            //execute the moves
            foreach (string line in InputBlocks[1])
            {
                int[] move = line.ExtractNumbers<int>();
                for (int i = 0; i < move[0]; i++)
                {
                    stacks[0].Push(stacks[move[1]].Pop());
                }
                while (stacks[0].Count> 0) stacks[move[2]].Push(stacks[0].Pop());
            }
            //get final code
            string code = new string(stacks.Skip(1).Select(x => x.Peek()).ToArray());
            Submit(code);
        }
    }
}
