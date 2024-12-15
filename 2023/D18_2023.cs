using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D18_2023 : Day
{
    public override void PartOne()
    {
        useExampleInput = true;
        Grid<char> grid = new Grid<char>('.');
        Point2D start = (0, 0);
        foreach (string line in Input)
        {
            string[] split = line.Split(' ');
            Direction direction = DirectionExt.FromChar(split[0][0]);
            int amount = int.Parse(split[1]);
            char c = direction.Vertical() ? '|' : '-';
            if (direction.North())
            {
                grid[start] = c;
            }
            for (int i = 0; i < amount; i++)
            {
                start = start.NextIn(direction);
                if (direction.North() && i + 1 == amount) grid[start] = '-';
                else grid[start] = c;
            }
        }
        bool insidePipe = false;
        foreach (Point2D point in grid.Scan())
        {
            if (grid.ContainsPoint(point))
            {
                if (grid[point] == '|') insidePipe = !insidePipe;
            }
            else if (insidePipe) grid[point] = '#';
        }
        if (useExampleInput) grid.PrintBoard();
        Submit(grid.Count);
    }
    public override void PartTwo()
    {
        //useExampleInput = true;
        int amount, perimeter = 0;
        long width = 1, area = 0;

        foreach (string line in Input)
        {
            Direction direction = LineParser(line, false, out amount);
            perimeter += amount;
            if (direction is Direction.North or Direction.West) amount = -amount;
            if (direction.Horizontal()) width += amount;
            else area += width * amount;
        }
        Submit(area + perimeter/2 + 1);
    }
    Direction LineParser(string line, bool partA, out int amount)
    {
        string[] split = line.Split();
        if (partA)
        {
            amount = int.Parse(split[1]);
            return DirectionExt.FromChar(line[0]);
        }
        amount = HexToDec(split[2][2..7]);
        return (Direction)((int.Parse(split[2][7..8]) + 1) % 4);
    }
    int HexToDec(string hex)
    {
        int dec = 0;
        foreach (char c in hex) dec = (dec << 4) + "0123456789abcdef".IndexOf(c);
        return dec;
    }
}
