using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D01_2015 : Day
    {
        public override void PartOne()
        {
            string input = GetInputForDay()[0]; //get puzzle input
            int floor = input.Count(c => c == '(') - input.Count(c => c == ')'); //solve
            Copy(floor); //copy answer to clipboard
            Submit(floor);
        }
        public override void PartTwo()
        {
            string input = GetInputForDay()[0]; //get puzzle input
            int floor = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(') floor++;
                else floor--;
                if (floor < 0) //if in basement
                {
                    Submit(i + 1); //off by 1 errors hahaha
                    break;
                }
            }
        }
    }
}
