using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D21_2023 : Day<char>
{
    public override void PartA()
    {
        useExampleInput = true;
        int steps = useExampleInput ? 7 : 64;
        Grid<char> grid = new Grid<char>(Input2D);
        Point2D start = grid.FirstOrDefault(x => x.Value == 'S').Key;
        #region Purely Visualisation
        foreach (var item in grid.Scan())
        {
            if (grid[item] == '#') continue;
            if (!item.Adjacent().Any(x => grid[x] != '#'))
            {
                grid[item] = '£';
                continue;
            }
            int dist = item.ManhattanDistanceTo(start);
            if (dist <= steps && dist % 2 == steps % 2) grid[item] = '@';
        }
        grid.PrintBoard();
        #endregion
        int area = grid.Scan()
            .Where(x => grid[x] != '#' && x.Adjacent().Any(y => grid[y] != '#'))
            .Select(x => x.ManhattanDistanceTo(start))
            .Count(x => x <= steps && x % 2 == steps % 2);
        Submit(area);
    }
    long ReachablePoints(Grid<char> grid, int steps)
    {
        Point2D start = grid.FirstOrDefault(x => x.Value == 'S').Key;
        grid.SetPoints('#', grid.Scan().Where(x => x.Adjacent().All(y => grid[y] == '#')));
        Dictionary<Point2D, int> dist = grid.Scan().Where(x => grid[x] != '#')
            .ToDictionary(k => k, c => c.ManhattanDistanceTo(start));

        int gridWidth = InputRaw.Length; //131
        int n = steps / gridWidth;  //202300
        int modStep = steps % gridWidth; // 65

        long oddSquares = n + ((n + 1) % 2);
        oddSquares *= oddSquares;
        long evenSquares = n + (n % 2);
        evenSquares *= evenSquares;

        int oddFull = dist.Count(x => x.Value % 2 == steps % 2);
        int evenFull = dist.Count(x => x.Value % 2 != steps % 2);
        int oddOuter = dist.Count(x => x.Value % 2 == steps % 2 && x.Value > modStep);
        int evenOuter = dist.Count(x => x.Value % 2 != steps % 2 && x.Value > modStep);

        long total = (oddSquares * oddFull) + (evenSquares * evenFull);
        if (n % 2 == 0) total += (evenOuter * n) - (oddOuter * (n + 1));
        else total += (oddOuter * n) - (evenOuter * (n + 1));
        return total;
    }
    public override void PartB()
    {
        //useExampleInput = true; 
        
        int steps = 26501365;
        if (useExampleInput) steps = 25;
        Grid<char> grid = new Grid<char>(Input2D);
        Submit(ReachablePoints(grid, steps));
        return;
    }
}
