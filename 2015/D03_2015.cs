using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D03_2015 : Day
    {
        public override void PartA()
        {
            string santaPath = GetInputForDay()[0];
            HashSet<Coord> visitedHouses = new HashSet<Coord>();
            Coord at = new(0, 0);
            visitedHouses.Add(at);
            foreach (char direction in santaPath)
            {
                at.Move(direction);
                visitedHouses.Add(at);
            }
            Submit(visitedHouses.Count);
        }
        public override void PartB()
        {
            string santaPath = GetInputForDay()[0];
            HashSet<Coord> visitedHouses = new HashSet<Coord>();
            Coord at = new(0, 0);
            Coord robot = new(0, 0);
            visitedHouses.Add(at);
            for (int i = 0; i < santaPath.Length; i++)
            {
                at.Move(santaPath[i++]);
                visitedHouses.Add(at);
                robot.Move(santaPath[i]);
                visitedHouses.Add(robot);
            }
            Submit(visitedHouses.Count);
        }
        struct Coord
        {
            public int X, Y;
            public Coord(int x, int y)
            {
                X = x;
                Y = y;
            }

            internal void Move(char direction)
            {
                if (direction == '^') Y++;
                else if (direction == 'v') Y--;
                else if (direction == '>') X++;
                else if (direction == '<') X--;
            }
        }
    }
}
