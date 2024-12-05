using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D04_2024 : Day<char>
{
    struct FoundWord
    {
        public Point2D direction;
        public Point2D Origin;
        public FoundWord(Point2D origin)
        {
            Origin = origin;
            direction = origin;
        }
    }

    const string XMAS = "XMAS";
    public override void PartA()
    {
        Grid<char> grid = new(Input2D);

        //find all Xs
        var points = grid.points.Where(x => x.Value == XMAS[0]).Select(x => new FoundWord(x.Key));

        //Duplicate for each direction
        points = SplitPerDirection(points);

        //Find all XMAS words
        for (int i = 1; i < XMAS.Length; i++)
        {
            points = points.Where(p => grid[(p.direction * i) + p.Origin] == XMAS[i]).ToList();
        }
        Submit(points.Count());
    }

    public override void PartB()
    {
        Grid<char> grid = new(Input2D);

        //find all As
        var points = grid.points.Where(x => x.Value == 'A');

        List<List<Point2D>> opposites = new()
        {
            new() { (1, 1), (-1, -1) },
            new() { (-1, 1), (1, -1) }
        };
        int xmases = 0;
        foreach (var point in points)
        {
            if (opposites.Select(o => o.Select(x => grid[point.Key + x]))
                .All(c => c.Contains('M') && c.Contains('S')))
            {
                xmases++;
            }
        }
        Submit(xmases);
    }
    private static IEnumerable<FoundWord> SplitPerDirection(IEnumerable<FoundWord> points)
    {
        foreach (var point in points)
        {
            foreach (Point2D adjacent in point.Origin.Surrounding())
            {
                yield return point with { direction = adjacent - point.Origin };
            }
        }
    }
}
