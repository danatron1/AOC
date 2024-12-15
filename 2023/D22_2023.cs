using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D22_2023 : Day
{
    class Brick
    {
        public string identity;
        enum Orientation
        {
            Vertical,
            HorizontalX,
            HorizontalY
        }
        public Point3D cornerA;
        public Point3D cornerB;
        public Brick[] supporting;
        public int supportedBy = 0;
        Orientation orientation;
        public Brick(string line)
        {
            identity = line;
            string[] split = line.Split('~');
            int[] A = split[0].Split(',').Select(int.Parse).ToArray();
            int[] B = split[1].Split(',').Select(int.Parse).ToArray();
            cornerA = (A[0], A[1], A[2]);
            cornerB = (B[0], B[1], B[2]);
            if (cornerA.Z != cornerB.Z) orientation = Orientation.Vertical;
            else orientation = cornerA.Y == cornerB.Y ? Orientation.HorizontalX : Orientation.HorizontalY;
        }

        internal void Settle(Brick[] otherBricks, int myIndex)
        {
            int moveDown = cornerA.Z - GetCubes().Select(x => NextBelow(x, otherBricks, myIndex)).Max();
            cornerA = cornerA.NextIn(Direction3D.Down, moveDown);
            cornerB = cornerB.NextIn(Direction3D.Down, moveDown);
        }
        private int NextBelow(Point3D point, Brick[] otherBricks, int myIndex)
        {
            while (point.Z > 0)
            {
                point = point.Down;
                for (int i = myIndex -1; i >= 0; i--)
                {
                    if (otherBricks[i].Occupies(point)) return point.Z + 1;
                }
            }
            return 1;
        }
        public bool Occupies(Point3D point)
        {
            return cornerA.X <= point.X && cornerA.Y <= point.Y && cornerA.Z <= point.Z
                && cornerB.X >= point.X && cornerB.Y >= point.Y && cornerB.Z >= point.Z;
        }
        public bool OccupiesSlice(int? x, int? y, int? z)
        {
            return (!x.HasValue || cornerA.X <= x && cornerB.X >= x)
                && (!y.HasValue || cornerA.Y <= y && cornerB.Y >= y)
                && (!z.HasValue || cornerA.Z <= z && cornerB.Z >= z);
        }
        private IEnumerable<Point3D> GetCubes()
        {
            for (int x = cornerA.X; x <= cornerB.X; x++)
                for (int y = cornerA.Y; y <= cornerB.Y; y++)
                    for (int z = cornerA.Z; z <= cornerB.Z; z++)
                        yield return (x, y, z);
        }

        internal void CountSupporting(Brick[] bricks, int index)
        {
            HashSet<Brick> support = new HashSet<Brick>();
            foreach (Point3D point in GetCubes())
            {
                int i = index;
                while (++i < bricks.Length)
                {
                    if (bricks[i].cornerA.Z == cornerA.Z)
                    {
                        index = i;
                        continue;
                    }
                    if (bricks[i].cornerA.Z > point.Z + 1) break;
                    if (bricks[i].Occupies(point.Up))
                    {
                        support.Add(bricks[i]);
                        bricks[i].supportedBy++;
                    }
                }
            }
            supporting = support.ToArray();
        }
    }
    void PrintBricks(Brick[] bricks)
    {
        Point3D maxs =
        (
            bricks.Select(c => c.cornerB.X).Max(),
            bricks.Select(c => c.cornerB.Y).Max(),
            bricks.Select(c => c.cornerB.Z).Max()
        );
        //Axis labels
        string line = "";
        line += new string(' ', (maxs.X) / 2) + "X" + new string(' ', (maxs.X+3) / 2);
        line += new string(' ', (maxs.Y) / 2) + "Y" + new string(' ', (maxs.Y+3) / 2);
        Console.WriteLine(line + line);
        //Axis numbers
        line  = string.Join("", Enumerable.Range(0, maxs.X+1)) + " "; 
        line += string.Join("", Enumerable.Range(0, maxs.Y+1)) + " ";
        line += string.Join("", Enumerable.Range(0, maxs.X+1).Reverse()) + " ";
        line += string.Join("", Enumerable.Range(0, maxs.Y+1).Reverse());
        Console.WriteLine(line);
        //bricks
        for (int z = maxs.Z; z > 0; z--)
        {
            line = "";
            for (int x = 0; x <= maxs.X; x++) line += (char)TopChar(bricks, x, null, z, false);
            line += " ";
            for (int y = 0; y <= maxs.Y; y++) line += (char)TopChar(bricks, null, y, z, true);
            line += " ";
            for (int x = maxs.X; x >= 0; x--) line += (char)TopChar(bricks, x, null, z, true);
            line += " ";
            for (int y = maxs.Y; y >= 0; y--) line += (char)TopChar(bricks, null, y, z, false);
            line += " " + z;
            if (z == (maxs.Z + 1) / 2) line += " Z";
            Console.WriteLine(line);
        }
        //base
        line = new string('-', maxs.X+1) + " " + new string('-', maxs.Y+1) + " ";
        Console.WriteLine(line + line + "0");

        int TopChar(Brick[] bricks, int? x, int? y, int z, bool useDeepest)
        {
            if (x is null && y is null) return '$';
            Brick[] relevantBricks = bricks.Where(b => b.OccupiesSlice(x, y, z)).ToArray();
            if (relevantBricks.Length == 0) return '.';
            if (relevantBricks.Length == 1) return 'A' + (Array.IndexOf(bricks, relevantBricks[0]) % 26);
            Brick topBrick;
            if (x.HasValue)
            {
                if (useDeepest) topBrick = relevantBricks.OrderByDescending(b => b.cornerB.Y).First();
                else topBrick = relevantBricks.OrderBy(b => b.cornerA.Y).First();
            }
            else
            {
                if (useDeepest) topBrick = relevantBricks.OrderByDescending(b => b.cornerB.X).First();
                else topBrick = relevantBricks.OrderBy(b => b.cornerA.X).First();
            }
            return 'A' + (Array.IndexOf(bricks, topBrick) % 26);
        }
    }
    public override void PartOne()
    {
        useExampleInput = true;
        Stopwatch sw = Stopwatch.StartNew();
        Brick[] bricks = Input.Select(l => new Brick(l)).OrderBy(x => x.cornerA.Z).ToArray();
        if (useExampleInput) PrintBricks(bricks);
        Console.WriteLine($"{sw.Elapsed} Settling bricks...");
        for (int i = 0; i < bricks.Length; i++)
        {
            bricks[i].Settle(bricks[..i], i);
        }
        bricks = bricks.OrderBy(x => x.cornerA.Z).ToArray();
        if (useExampleInput) PrintBricks(bricks);
        Console.WriteLine($"{sw.Elapsed} Counting supports...");
        for (int i = 0; i < bricks.Length; i++)
        {
            bricks[i].CountSupporting(bricks, i);
            //Console.WriteLine($"Brick {bricks[i].identity} supports {bricks[i].supporting.Length} and is supported by {bricks[i].supportedBy}");
        }
        Submit(bricks.Count(x => x.supporting.All(y => y.supportedBy > 1)));
    }
    public override void PartTwo()
    {
        throw new NotImplementedException();
    }
}
