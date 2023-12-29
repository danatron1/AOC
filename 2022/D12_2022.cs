using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D12_2022 : Day<char>
    {
        public override void PartA()
        {
            Grid<char> grid = new(Input2D);
            Point2D start = grid.FindPoint('S')!.Value;
            Point2D end = grid.FindPoint('E')!.Value;
            grid[start] = 'a';
            grid[end] = 'z';
            GridPathfinder<char> pf = new(grid, end, (a, b, _) => a + 1 >= b);
            if (pf.Reach(start, out var found))
            {
                Console.WriteLine($"Found solution in {found.Cost} steps");
                Submit(found.Cost);
                while (true)
                {
                    if (found.previous is null) break;
                    grid[found.Point] = '@';
                    found = found.previous;
                }
            }
            else Console.WriteLine("No solution found");
            grid.PrintBoard();
        }
        public override void PartB()
        {
            Grid<char> grid = new(Input2D);
            Point2D start = grid.FindPoint('E')!.Value;
            Point2D end = grid.FindPoint('S')!.Value;
            grid[start] = 'z';
            grid[end] = 'a';
            GridPathfinder<char> pf = new(grid, e => grid[e] == 'a', (a, b, _) => a - 1 <= b);
            if (pf.Reach(start, out var found))
            {
                Console.WriteLine($"Found solution in {found.Cost} steps");
                Submit(found.Cost);
                while (true)
                {
                    if (found.previous is null) break;
                    grid[found.Point] = '@';
                    found = found.previous;
                }
            }
            else Console.WriteLine("No solution found");
            grid.PrintBoard();
        }
    }
}
