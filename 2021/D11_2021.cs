using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D11_2021 : Day
    {
        int[,] octopus;
        public override void Solve() => PartB();
        public override void PartA()
        {
            octopus = GetInputForDay2D<int>();
            int flashed = 0;
            for (int steps = 0; steps < 100; steps++)
            {
                Console.WriteLine($"After iteration {steps}, {flashed} flashed total:");
                PrintBoard();
                Console.ReadLine();
                Console.Clear();
                AdvanceStep();
                foreach (int o in octopus)
                {
                    if (o == 0) flashed++;
                }
            }
            Copy(flashed);
        }
        void PrintBoard()
        {
            for (i = 0; i < octopus.Length; i++)
            {
                if (octopus[x, y] == 0) Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(octopus[x, y]);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (x + 1 == octopus.GetLength(1)) Console.WriteLine();
            }
        }
        int i;
        int y => i / octopus.GetLength(1);
        int x => i % octopus.GetLength(1);
        void AdvanceStep()
        {
            Queue<int> toFlash = new Queue<int>();
            //increase all by 1
            for (i = 0; i < octopus.Length; i++)
            {
                octopus[x, y]++;
                if (octopus[x, y] == 10) toFlash.Enqueue(i);
            }
            //initiate flashing
            while (toFlash.Count > 0)
            {
                i = toFlash.Dequeue();
                for (int X = -1; X <= 1; X++)
                {
                    if (x + X < 0 || x + X >= octopus.GetLength(0)) continue;
                    for (int Y = -1; Y <= 1; Y++)
                    {
                        if (y + Y < 0 || y + Y >= octopus.GetLength(1)) continue;
                        octopus[x + X, y + Y]++;
                        int thisOct = (y + Y) * octopus.GetLength(1) + X + x;
                        if (octopus[x + X, y + Y] == 10) toFlash.Enqueue(thisOct);
                    }
                }
            }
            //reset the flashers
            for (i = 0; i < octopus.Length; i++)
            {
                if (octopus[x, y] > 9) octopus[x, y] = 0;
            }
        }
        public override void PartB()
        {
            octopus = GetInputForDay2D<int>();
            int lastFlashed = 0, flashed = 0;
            int steps = 0;
            while (flashed - octopus.Length != lastFlashed)
            {
                steps++;
                lastFlashed = flashed;
                Console.Clear();
                AdvanceStep();
                foreach (int o in octopus)
                {
                    if (o == 0) flashed++;
                }
                Console.WriteLine($"After iteration {steps}, {flashed} flashed total:");
                PrintBoard();
                Thread.Sleep(100);
            }
            Copy(steps);
        }
    }
}
