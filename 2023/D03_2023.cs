using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D03_2023 : Day<char>
{
    class PartNumber
    {
        internal bool HasAdjacentSymbol;
        Point2D coords;
        int digits;
        internal int number;

        public PartNumber(KeyValuePair<Point2D, char> point, Grid<char> inputGrid)
        {
            coords = point.Key;
            number = 0;
            Point2D read = coords;
            while (char.IsDigit(inputGrid[read]))
            {
                number *= 10;
                number += inputGrid[read] - '0';
                digits++;
                read = read.East;
            }

            read = coords;
            HasAdjacentSymbol = false;
            for (int i = 0; i < digits; i++)
            {
                if (read.Surrounding().Any(x => !char.IsDigit(inputGrid[x]) && inputGrid[x] != inputGrid.DefaultValue))
                {
                    HasAdjacentSymbol = true;
                    break;
                }
                read = read.East;
            }
        }

        internal void LogTo(Dictionary<Point2D, PartNumber> partLog)
        {
            Point2D point = coords;
            for (int i = 0; i < digits; i++)
            {
                partLog[point] = this;
                point = point.East;
            }
        }
    }
    public override void PartOne()
    {
        Grid<char> inputGrid = new(Input2D, '.');
        var nums = inputGrid.Where(x => char.IsDigit(x.Value) && !char.IsDigit(inputGrid[x.Key.West]));
        PartNumber[] parts = nums.Select(x => new PartNumber(x, inputGrid)).ToArray();
        Submit(parts.Where(x => x.HasAdjacentSymbol).Sum(x => x.number));
    }
    public override void PartTwo()
    {
        Dictionary<Point2D, PartNumber> partLog = new();
        Grid<char> inputGrid = new(Input2D, '.');
        var nums = inputGrid.Where(x => char.IsDigit(x.Value) && !char.IsDigit(inputGrid[x.Key.West]));
        PartNumber[] parts = nums.Select(x => new PartNumber(x, inputGrid)).ToArray();
        foreach (PartNumber part in parts) part.LogTo(partLog);
        var potentialGears = inputGrid.Where(x => x.Value == '*');
        var gears = potentialGears.Where(x => x.Key.Surrounding().Where(partLog.ContainsKey).Select(x => partLog[x]).Distinct().Count() == 2);
        Submit(gears.Sum(x => x.Key.Surrounding().Where(partLog.ContainsKey).Select(x => partLog[x]).Distinct().Mul(x => x.number)));
    }
}
