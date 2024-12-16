using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D08_2024 : Day<char>
{
    private static IEnumerable<char> Frequencies()
    {
        for (char c = '0'; c <= '9'; c++) yield return c;
        for (char c = 'a'; c <= 'z'; c++) yield return c;
        for (char c = 'A'; c <= 'Z'; c++) yield return c;
    }
    public override void PartOne()
    {
        Grid<char> grid = new(Input2D);
        Dictionary<char, HashSet<Point2D>> pointMap = new()
        {
            { '#', new() } //antinode points
        };
        foreach (char c in Frequencies())
        {
            pointMap.Add(c, grid.Where(x => x.Value == c).Select(x => x.Key).ToHashSet());
            foreach (var pair in pointMap[c].Pairs(orderMatters: true))
            {
                Point2D antinode = pair[0] + (pair[0] - pair[1]);
                if (grid.ContainsPoint(antinode)) pointMap['#'].Add(antinode);
            }
        }
        Submit(pointMap['#'].Count);
    }
    public override void PartTwo()
    {
        Grid<char> grid = new(Input2D);
        Dictionary<char, HashSet<Point2D>> pointMap = new()
        {
            { '#', new() } //antinode points
        };
        foreach (char c in Frequencies())
        {
            pointMap.Add(c, grid.Where(x => x.Value == c).Select(x => x.Key).ToHashSet());
            foreach (var pair in pointMap[c].Pairs(orderMatters: true))
            {
                Point2D antinode = pair[0], pointDiff = pair[0] - pair[1];
                do
                {
                    pointMap['#'].Add(antinode);
                    antinode += pointDiff;
                }
                while (grid.ContainsPoint(antinode));
            }
        }
        Submit(pointMap['#'].Count);
    }
}
