using AOC.Items.Geometry;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AOC.Y2023;

internal class D16_2023 : Day<char>
{
    class MirrorField
    {
        public const string elementChars = "\\/-|";
        public Grid<char> grid;
        public HashSet<Point2D> energised;
        public HashSet<int> seenInteractions;
        public Dictionary<Point2D, int> elementID;
        public List<(Point2D? a, Point2D? b)> interactions;
        public MirrorField(Grid<char> grid)
        {
            this.grid = grid;
            ClearEnergised();
            ClearCaching();
        }
        void ClearCaching()
        {
            elementID = new();
            interactions = new();
        }
        void ClearEnergised()
        {
            energised = new();
            seenInteractions = new();
        }

        internal int StartFrom(Point2D start, Direction dir)
        {
            ClearEnergised();
            Energise(start, NextRelevantPoint(start, dir));
            return energised.Count;
        }
        void Energise(Point2D from, Point2D? to)
        {
            if (to is null) return;
            Direction? dir = to.Value.DirectionFrom(from);
            energised.UnionWith(Grid<char>.PointsInArea(from.NextIn(dir), to.Value));
            AddEnergisedFrom(to.Value, dir);
        }
        Point2D NextRelevantPoint(Point2D start, Direction direction)
        {
            Point2D relevantPoint = start;
            foreach (Point2D point in grid.WalkToEdge(start, direction))
            {
                relevantPoint = point;
                if (elementChars.Contains(grid[point])) break;
            }
            return relevantPoint;
        }
        IEnumerable<Direction> ReflectHow(char from, Direction inDir)
        {
            //(Direction a, Direction? b) answer = (inDir, null);
            if (from == '/') yield return inDir.Vertical() ? inDir.Right() : inDir.Left();
            else if (from == '\\') yield return inDir.Horizontal() ? inDir.Right() : inDir.Left();
            else if (from == '-')
            {
                if (inDir.Horizontal()) yield return inDir;
                else
                {
                    yield return inDir.Left();
                    yield return inDir.Right();
                }
            }
            else if (from == '|')
            {
                if (inDir.Vertical()) yield return inDir; 
                else
                {
                    yield return inDir.Left();
                    yield return inDir.Right();
                }
            }
        }
        void GenerateBounceFrom(Point2D start, Direction direction)
        {
            (Point2D? a, Point2D? b) next = (null, null);
            bool onA = true;
            foreach (Direction d in ReflectHow(grid[start], direction))
            {
                if (onA) next.a = NextRelevantPoint(start, d);
                else next.b = NextRelevantPoint(start, d);
                onA = false;
            }
            interactions.Add(next);
        }
        private void AddEnergisedFrom(Point2D start, Direction? direction)
        {
            if (direction is null || !elementChars.Contains(grid[start])) return;
            if (!elementID.ContainsKey(start))
            {
                elementID.Add(start, interactions.Count);
                foreach (Direction d in DirectionExt.All())
                {
                    GenerateBounceFrom(start, d);
                }
            }
            int id = elementID[start] + (int)direction.Value;
            if (seenInteractions.Contains(id)) return;
            seenInteractions.Add(id);
            var (a, b) = interactions[id];
            Energise(start, a);
            Energise(start, b);
        }
    }
    public override void PartA()
    {
        //useExampleInput = true;
        MirrorField mirrors = new MirrorField(new Grid<char>(Input2D, '.'));
        Submit(mirrors.StartFrom(mirrors.grid.GetCorners().TopLeft.West, Direction.East));
    }
    public override void PartB()
    {
        //useExampleInput = true;
        MirrorField mirrors = new MirrorField(new Grid<char>(Input2D, '.'));
        int best = 0;
        var corners = mirrors.grid.GetCorners();
        best = Math.Max(best, mirrors.grid.PointsInColumn(corners.TopLeft.West).Max(p => mirrors.StartFrom(p, Direction.East)));
        best = Math.Max(best, mirrors.grid.PointsInRow(corners.BottomLeft.South).Max(p => mirrors.StartFrom(p, Direction.North)));
        best = Math.Max(best, mirrors.grid.PointsInColumn(corners.TopRight.East).Max(p => mirrors.StartFrom(p, Direction.West)));
        best = Math.Max(best, mirrors.grid.PointsInRow(corners.TopLeft.North).Max(p => mirrors.StartFrom(p, Direction.South)));
        Submit(best);
    }
}
