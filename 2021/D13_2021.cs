using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D13_2021 : Day
    {
        class TransparentSheet
        {
            int neededSize => Math.Max(realX, realY);
            int realX;
            int realY;
            public bool[,] grid;
            public TransparentSheet(string[] pointPairs, int X = 0, int Y = 0)
            {
                if (X == 0) X = pointPairs.Select(s => int.Parse(s[..s.IndexOf(',')])).Max() + 1;
                if (Y == 0) Y = pointPairs.Select(s => int.Parse(s[(s.IndexOf(',')+1)..])).Max() + 1;
                realY = Y;
                realX = X;
                Console.WriteLine($"X: {X}, Y: {Y}");
                grid = new bool[realX,realY];
                foreach (string p in pointPairs)
                {
                    string[] s = p.Split(',');
                    grid[int.Parse(s[0]), int.Parse(s[1])] = true;
                }
            }
            public void FoldX(int along = 0)
            {
                if (along == 0) along = realX / 2;
                Console.WriteLine($"Folding along {along} on board size {realX}/{realY}");
                for (int y = 0; y < realY; y++)
                {
                    for (int x = along + 1; x <= along * 2; x++)
                    {
                        if (x >= grid.GetLength(0) || !grid[x, y]) continue;
                        grid[2 * along - x, y] = true;
                        grid[x, y] = false;
                    }
                }
                realX = along;
            }
            public void FoldY(int along = 0)
            {
                if (along == 0) along = realY / 2;
                Console.WriteLine($"Folding along {along} on board size {realX}/{realY}");
                for (int y = along + 1; y <= along * 2; y++)
                {
                    for (int x = 0; x < realX; x++)
                    {
                        if (y >= grid.GetLength(1) || !grid[x, y]) continue;
                        grid[x, 2 * along - y] = true;
                        grid[x, y] = false;
                    }
                }
                realY = along;
            }
            public void PrintBoard(int step)
            {
                int totalDots = 0;
                using (Bitmap b = new Bitmap(realX, realY))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.Transparent);
                        Brush brush = new SolidBrush(Color.Black);
                        for (int y = 0; y < realY; y++)
                        {
                            string s = "";
                            for (int x = 0; x < realX; x++)
                            {
                                if (grid[x, y])
                                {
                                    s += '█';
                                    totalDots++;
                                    g.FillRectangle(brush, x, y, 1, 1);
                                }
                                else s += ' ';
                            }
                            if (realX * realY < 2000) Console.WriteLine(s);

                        }
                    }
                    string path = @"F:\Documents\AdventOfCode\2021\D13Visual";
                    Directory.CreateDirectory(path);
                    b.Save($"{path}\\Step{step}.png", ImageFormat.Png);
                }
                Console.WriteLine("Total dots: " + totalDots);
            }
        }
        public override void PartA()
        {
            //825 too high
            string[] input = GetInputForDay(example: false);

            string[] instructions = input.Where(c => c.Length > 0 && c[0] == 'f').ToArray();
            string[] points = input.Where(c => c.Length > 0 && char.IsDigit(c[0])).ToArray();
            Console.WriteLine($"Okay there's definitely {points.Length} dots");

            TransparentSheet sheet = new TransparentSheet(points);
            int i = 0;
            sheet.PrintBoard(i); //starting board
            foreach (string instruction in instructions)
            {
                int value = int.Parse(instruction[(instruction.IndexOf('=')+1)..]);
                if (instruction.Contains('y')) sheet.FoldY(value);
                else sheet.FoldX(value);
                sheet.PrintBoard(++i);
            }
        }

        public override void PartB()
        {
            throw new NotImplementedException();
        }

        public override void Solve() => PartA();
    }
}
