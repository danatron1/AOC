using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AOC.D3_2019;
using static AOC.Items.Geometry.Point2D;

namespace AOC.Y2022
{
    internal class D09_2022 : Day
    {
        class Rope
        {
            public Point2D Location = new();
            public Rope Parent;
            public List<Rope> Children = new List<Rope>();
            public Rope(Rope parent)
            {
                Parent = parent;
                Parent?.Children.Add(this);
            }
            public void Move(params char[] dir)
            {
                foreach (char c in dir) Location = Location.NextIn(DirectionExt.FromChar(c));
                foreach (Rope r in Children) r.ChaseParent();
            }
            public void ChaseParent()
            {
                if (Parent.Location.Surrounding(true).Contains(Location)) return;
                string toMove = "";
                if (Parent.Location.X < Location.X) toMove += "L";
                if (Parent.Location.X > Location.X) toMove += "R";
                if (Parent.Location.Y < Location.Y) toMove += "D";
                if (Parent.Location.Y > Location.Y) toMove += "U";
                Move(toMove.ToCharArray());
            }
        }
        HashSet<Point2D> SimulateRope(int knots) //owo
        {
            Rope[] rope = new Rope[knots];
            rope[0] = new(null);
            for (int i = 1; i < rope.Length; i++) rope[i] = new(rope[i - 1]);
            HashSet<Point2D> Path = new();
            foreach (string line in Input)
            {
                int times = line.ExtractNumber<int>();
                for (int i = 0; i < times; i++)
                {
                    rope[0].Move(line[0]);
                    Path.Add(rope[^1].Location);
                }
            }
            return Path;
        }
        public override void PartA()
        {
            Rope head = new(null);
            Rope tail = new(head);
            HashSet<Point2D> Path = new();

            //Grid<int> drawnPath = new Grid<int>();
            //drawnPath.printedRepresentation.Add(0, ".");
            //drawnPath.printedRepresentation.Add(1, "#");

            foreach (string line in Input)
            {
                int times = line.ExtractNumber<int>();
                for (int i = 0; i < times; i++)
                {
                    head.Move(line[0]);
                    Path.Add(tail.Location);

                    //drawnPath[tail.Location] = 1;
                }
            }
            //drawnPath.PrintBoard();

            Submit(Path.Count);
        }
        public override void PartB()
        {
            HashSet<Point2D> visited = SimulateRope(10);
            Grid<bool> drawnPath = new(true, visited);
            drawnPath.PrintBoard();
            Submit(visited.Count);
        }
    }
}
