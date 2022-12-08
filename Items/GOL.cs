using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;
internal class GOL
{
    public Grid<bool> grid = new();
    public int Lifetime = 0;
    public GOL() : this("3", "23") { }
    public GOL(string rules) : this((int)rules.ExtractNumbers()[0], (int)rules.ExtractNumbers()[1]) { }
    public GOL(int born, int stay) : this(born.Digits(), stay.Digits()) { }
    public GOL(string born, string stay) : this(born.Digits(), stay.Digits()) { }
    public GOL(int[] born, int[] stay)
    {
        this.born = born;
        this.stay = stay;
        grid.DefaultValue = false;
        grid.printedRepresentation.Add(true, "#");
        grid.printedRepresentation.Add(false, ".");
    }
    private readonly int[] born, stay; //Typical GOL rules; B23/S3
    public bool ShouldLive(Point2D p)
    {
        return (grid[p] ? stay : born).Contains(p.Surrounding().Where(s => grid[s]).Count());
    }
    public void Iterate(int times = 1, bool printBoards = false, bool manualStepThrough = false)
    {
        if (printBoards)
        {
            Console.WriteLine($"Iteration 0");
            grid.PrintBoard();
            if (manualStepThrough) Console.ReadLine();
        }
        for (int i = 0; i < times; i++)
        {
            Lifetime++;
            Point2D[] changed = grid.GetPoints().WithSurrounding().Distinct()
                .Where(p => ShouldLive(p) != grid[p]).ToArray();
            foreach (Point2D item in changed)
            {
                grid[item] = !grid[item];
            }
            if (printBoards)
            {
                Console.WriteLine($"\nIteration {Lifetime}");
                grid.PrintBoard();
                if (manualStepThrough) Console.ReadLine();
            }
        }
    }
}
public static class GOLExt
{
    public static IEnumerable<Point2D> WithSurrounding(this IEnumerable<Point2D> points)
    {
        return points.SelectMany(p => p.Surrounding(true));
    }
}
