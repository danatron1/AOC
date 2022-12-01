using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D2_2021 : Day
    {
        public override void Solve() => PartB();
        public override void PartA()
        {
            string[] input = GetInputForDay(); //get input
            int horizontal = 0;
            int depth = 0;
            foreach (string item in input)
            {
                string[] line = item.Split(' ');
                int amount = int.Parse(line[1]);
                switch (line[0])
                {
                    case "forward":
                        horizontal += amount;
                        break;
                    case "up":
                        depth -= amount;
                        break;
                    case "down":
                        depth += amount;
                        break;
                }
            }
            Submit(horizontal * depth); //copy result to clipboard
        }
        public override void PartB()
        {
            string[] input = GetInputForDay(); //get input
            int aim = 0;
            int horizontal = 0;
            int depth = 0;
            foreach (string item in input)
            {
                string[] line = item.Split(' ');
                int amount = int.Parse(line[1]);
                switch (line[0])
                {
                    case "forward":
                        horizontal += amount;
                        depth += amount * aim;
                        break;
                    case "up":
                        aim -= amount;
                        break;
                    case "down":
                        aim += amount;
                        break;
                }
            }
            Submit(horizontal * depth); //copy result to clipboard
        }
    }
}
