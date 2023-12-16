using AOC.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D11_2023 : Day<char>
{
    int[] GetEmptyColumns(Grid<char> grid) => grid.Columns().Where(x => !grid.ValuesInColumn(x).Any()).ToArray();
    int[] GetEmptyRows(Grid<char> grid) => grid.Rows().Where(x => !grid.ValuesInRow(x).Any()).ToArray();
    Point2D GalacticCell(int[] emptyColumns, int[] emptyRows, Point2D point)
    {
        return (emptyColumns.Count(x => x < point.X), emptyRows.Count(y => y < point.Y));
    }
    public override void PartA()
    {
        //useExampleInput = true;
        Grid<char> grid = new Grid<char>(Input2D, '.');
        int[] emptyC = GetEmptyColumns(grid);
        int[] emptyR = GetEmptyRows(grid);
        IEnumerable<Point2D> points = grid.GetPoints().Select(p => p + GalacticCell(emptyC, emptyR, p));
        Submit(points.PairsAlt().Sum(p => p.First().ManhattanDistanceTo(p.Last())));
    }
    public override void PartB()
    {
        //useExampleInput = true;
        Grid<char> grid = new Grid<char>(Input2D, '.');
        int[] emptyC = GetEmptyColumns(grid);
        int[] emptyR = GetEmptyRows(grid);
        IEnumerable<Point2D> points = grid.GetPoints().Select(p => p + 999_999 * GalacticCell(emptyC, emptyR, p));
        Submit(points.PairsAlt().Sum(p => (long)p.First().ManhattanDistanceTo(p.Last())));
    }
}
