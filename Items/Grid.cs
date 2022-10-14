using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;
public class Grid<T> where T : notnull
{
    private Dictionary<Point2D, T> points = new();
    public Dictionary<T, string> printedRepresentation = new();
    public int? MaxX, MaxY, MinX, MinY;
    public T defaultValue = default!;
    public void SetLimits(int? maxX = null, int? maxY = null, int? minX = 0, int? minY = 0)
    {
        MaxX = maxX;
        MaxY = maxY;
        MinX = minX;
        MinY = minY;
    }
    public void SetPoint(Point2D p, T value)
    {
        if (p.X >= MaxX || p.X < MinX || p.Y >= MaxY || p.Y < MinY) return;
        if (points.ContainsKey(p))
        {
            if (value is null || value.Equals(defaultValue)) points.Remove(p);
            else points[p] = value;
        }
        else if (!(value is null || value.Equals(defaultValue))) points.Add(p, value);
    }
    public T GetPoint(Point2D p)
    {
        if (points.TryGetValue(p, out T v)) return v;
        else return defaultValue;
    }
    public IEnumerable<Point2D> GetPoints() => points.Select(s => s.Key);
    public IEnumerable<T> GetValues() => points.Select(s => s.Value);
    public IEnumerable<(Point2D point, T value)> GetPointsAndValues() => points.Select(s => (s.Key, s.Value));
    public void SetPoints(T[,] input)
    {
        for (int y = 0; y < input.GetLength(1); y++)
        {
            for (int x = 0; x < input.GetLength(0); x++)
            {
                SetPoint((x, y), input[x, y]);
            }
        }
    }
    public T this[Point2D p]
    {
        get => GetPoint(p);
        set => SetPoint(p, value);
    }
    public T this[int x, int y]
    {
        get => this[(x, y)];
        set => this[(x, y)] = value;
    }

    public void PrintBoard()
    {
        var limits = GetLimitsOrExtremes();
        for (int y = limits.minY; y < limits.maxY; y++)
        {
            for (int x = limits.minX; x < limits.maxX; x++)
            {
                T? point = GetPoint((x, y));
                if (point is null) Console.Write("NULL");
                else if (printedRepresentation.TryGetValue(point, out string? rep)) Console.Write(rep);
                else Console.Write(point);
            }
            Console.WriteLine();
        }
    }
    public (int maxX, int maxY, int minX, int minY) GetLimitsOrExtremes()
    {
        (int maxX, int maxY, int minX, int minY) output;
        output.maxX = MaxX ?? points.MaxBy(x => x.Key.X).Key.X + 1;
        output.maxY = MaxY ?? points.MaxBy(y => y.Key.Y).Key.Y + 1;
        output.minX = MinX ?? points.MinBy(x => x.Key.X).Key.X;
        output.minY = MinY ?? points.MinBy(y => y.Key.Y).Key.Y;
        return output;
    }

    internal Point2D[] GetCorners()
    {
        var limits = GetLimitsOrExtremes();
        return new Point2D[]
        {
            (limits.minX,limits.minY),
            (limits.minX,limits.maxY-1),
            (limits.maxX-1,limits.minY),
            (limits.maxX-1,limits.maxY-1)
        };
    }
}

