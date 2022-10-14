using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D9_2021 : Day
    {
        public override void Solve() => PartB();
        byte[,] caveMap;
        public override void PartA()
        {
            GetCaveMap();
            int riskLevel = 0;
            for (int x = 0; x < caveMap.GetLength(0); x++)
            {
                for (int y = 0; y < caveMap.GetLength(1); y++)
                {
                    if (x + 1 < caveMap.GetLength(0) && caveMap[x + 1, y] <= caveMap[x, y]) continue;
                    if (y + 1 < caveMap.GetLength(1) && caveMap[x, y + 1] <= caveMap[x, y]) continue;
                    if (x > 0 && caveMap[x - 1, y] <= caveMap[x, y]) continue;
                    if (y > 0 && caveMap[x, y - 1] <= caveMap[x, y]) continue;
                    riskLevel += caveMap[x, y] + 1;
                }
            }
            Copy(riskLevel);
            //7177 too high
        }
        void GetCaveMap()
        {
            string[] input = GetInputForDay();
            caveMap = new byte[input[0].Length, input.Length];
            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[0].Length; x++)
                {
                    caveMap[x, y] = byte.Parse(input[y][x].ToString());
                }
            }
        }

        public override void PartB()
        {
            GetCaveMap();
            List<XY> lowPoints = new List<XY>();
            for (int x = 0; x < caveMap.GetLength(0); x++)
            {
                for (int y = 0; y < caveMap.GetLength(1); y++)
                {
                    if (x + 1 < caveMap.GetLength(0) && caveMap[x + 1, y] <= caveMap[x, y]) continue;
                    if (y + 1 < caveMap.GetLength(1) && caveMap[x, y + 1] <= caveMap[x, y]) continue;
                    if (x > 0 && caveMap[x - 1, y] <= caveMap[x, y]) continue;
                    if (y > 0 && caveMap[x, y - 1] <= caveMap[x, y]) continue;
                    lowPoints.Add(new XY(x, y));
                }
            }
            List<int> basinSizes = new List<int>();
            Console.WriteLine($"{lowPoints.Count} low points found total");
            foreach (XY low in lowPoints)
            {
                HashSet<XY> basin = new HashSet<XY>() { low };
                HashSet<XY> prevBasin = new HashSet<XY>();
                do
                {
                    prevBasin.UnionWith(basin);
                    foreach (XY p in prevBasin)
                    {
                        if (p.fullyChecked) continue;
                        if (p.X + 1 < caveMap.GetLength(0) && caveMap[p.X + 1, p.Y] < 9) basin.Add(new XY(p.X + 1, p.Y));
                        if (p.Y + 1 < caveMap.GetLength(1) && caveMap[p.X, p.Y + 1] < 9) basin.Add(new XY(p.X, p.Y + 1));
                        if (p.X > 0 && caveMap[p.X - 1, p.Y] < 9) basin.Add(new XY(p.X - 1, p.Y));
                        if (p.Y > 0 && caveMap[p.X, p.Y - 1] < 9) basin.Add(new XY(p.X, p.Y - 1));
                    }
                } while (basin.Count > prevBasin.Count);
                Console.WriteLine($"Finished finding new spots in basin {basinSizes.Count}/{lowPoints.Count}, adding size {basin.Count} to list!");
                basinSizes.Add(basin.Count);
            }
            basinSizes.Sort();
            basinSizes.Reverse();
            Console.WriteLine("3 biggest: \n" + basinSizes[0]);
            Console.WriteLine(basinSizes[1]);
            Console.WriteLine(basinSizes[2]);
            Copy(basinSizes[0] * basinSizes[1] * basinSizes[2]);
        }
        struct XY
        {
            public int X;
            public int Y;
            public bool fullyChecked;
            public XY(int x, int y)
            {
                X = x;
                Y = y;
                fullyChecked = false;
            }
        }
    }
}
