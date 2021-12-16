using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D5_2021 : Day
    {
        public override void Solve() => PartB();
        public override void PartA()
        {
            string[] input = GetInputForDay();
            Tally<string> tally = new Tally<string>();
            foreach (string item in input)
            {
                string[] coordinates = item.Split(" -> ");
                string[] start = coordinates[0].Split(',');
                string[] end = coordinates[1].Split(',');
                if (start[0] == end[0])
                {
                    int X = int.Parse(start[0]);
                    int startY = int.Parse(start[1]);
                    int endY = int.Parse(end[1]);
                    while (startY != endY)
                    {
                        tally[$"{X},{startY}"]++;
                        if (startY < endY) startY++;
                        else startY--;
                    }
                    tally[$"{X},{endY}"]++;
                }
                else if (start[1] == end[1])
                {
                    int Y = int.Parse(start[1]);
                    int startX = int.Parse(start[0]);
                    int endX = int.Parse(end[0]);
                    while (startX != endX)
                    {
                        tally[$"{startX},{Y}"]++;
                        if (startX < endX) startX++;
                        else startX--;
                    }
                    tally[$"{endX},{Y}"]++;
                }
            }
            Copy(tally.Count(o => tally[o] > 1));
        }

        public override void PartB()
        {
            Tally<string> tally = new Tally<string>();
            string[] input = GetInputForDay();
            foreach (string item in input)
            {
                string[] coordinates = item.Split(" -> ");
                string[] points = AllPointsBetween(coordinates[0], coordinates[1]);
                foreach (string point in points)
                {
                    tally[point]++;
                }
            }
            Copy(tally.CountAll(2)); //this is where I get the answer
            //everything below is just for the sake of my own curiosity.
            foreach (string item in tally)
            {
                if (tally[item] < 2) continue;
                Console.WriteLine($"{tally[item]}: {item}");
            }
        }

        string[] AllPointsBetween(string start, string end)
        {
            List<string> points = new List<string>();
            int startX = int.Parse(start.Split(',')[0]);
            int startY = int.Parse(start.Split(',')[1]);
            int endX = int.Parse(end.Split(',')[0]);
            int endY = int.Parse(end.Split(',')[1]);
            int steps = 1 + Math.Max(Math.Abs(startX - endX), Math.Abs(startY-endY));
            for (int i = 0; i < steps; i++)
            {
                int thisX = startX < endX ? startX + i : startX - i;
                int thisY = startY < endY ? startY + i : startY - i;
                if (startX == endX) thisX = startX;
                if (startY == endY) thisY = startY;
                points.Add($"{thisX},{thisY}");
            }
            return points.ToArray();
        }
    }
}
