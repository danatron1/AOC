using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;
public class Grid<T> : ICloneable, IEnumerable<KeyValuePair<Point2D, T>> where T : notnull
{
    private Dictionary<Point2D, T> points = new();
    public Dictionary<T, string> printedRepresentation = new();
    public int? MaxX, MaxY, MinX, MinY;
    public T DefaultValue = default!;
    public Grid() { }
    public Grid(IEnumerable<KeyValuePair<Point2D, T>> grid)
    {
        foreach (var item in grid)
        {
            points.Add(item.Key, item.Value);
        }
    }
    public Grid(T defaultValue)
    {
        DefaultValue= defaultValue;
    }
    public Grid(T[,] input2d)
    {
        SetPoints(input2d);
    }
    public Grid(T[,] input2d, T defaultValue) : this(defaultValue)
    {
        SetPoints(input2d);
    }
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
            if (value is null || value.Equals(DefaultValue)) points.Remove(p);
            else points[p] = value;
        }
        else if (!(value is null || value.Equals(DefaultValue))) points.Add(p, value);
    }
    public T GetPoint(Point2D p)
    {
        if (points.TryGetValue(p, out T v)) return v;
        else return DefaultValue;
    }
    public IEnumerable<Point2D> GetPoints() => points.Select(s => s.Key);
    public IEnumerable<T> GetValues() => points.Select(s => s.Value);
    public IEnumerable<(Point2D point, T value)> GetPointsAndValues() => points.Select(s => (s.Key, s.Value));
    public T[,] To2DArray()
    {
        var limits = GetLimitsOrExtremes();
        T[,] array = new T[limits.maxX, limits.maxY];
        foreach (Point2D item in GetPoints())
        {
            array[item.X, item.Y] = GetPoint(item);
        }
        return array;
    }
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
    internal Grid<T> PointsFrom(Point2D point, Point2D.Direction direction, bool lineOnly)
    {
        return PointsFrom(point.X, point.Y, direction, lineOnly);
    }
    internal Grid<T> PointsFrom(int x, int y, Point2D.Direction direction, bool lineOnly)
    {
        switch (direction)
        {
            case Point2D.Direction.North: return lineOnly ? PointsInArea(x, x, null, y - 1) : PointsInArea(null, null, null, y - 1);
            case Point2D.Direction.East:  return lineOnly ? PointsInArea(x + 1, null, y, y) : PointsInArea(x + 1, null, null, null);
            case Point2D.Direction.South: return lineOnly ? PointsInArea(x, x, y + 1, null) : PointsInArea(null, null, y + 1, null);
            case Point2D.Direction.West:  return lineOnly ? PointsInArea(null, x - 1, y, y) : PointsInArea(null, x - 1, null, null);
        }
        return new Grid<T>();
    }
    internal Grid<T> PointsInArea(int? xLeft, int? xRight, int? yTop, int? yBottom)
    {
        return new Grid<T>(points.Where(p =>
            (!xLeft.HasValue || p.Key.X >= xLeft)
         && (!xRight.HasValue || p.Key.X <= xRight)
         && (!yTop.HasValue || p.Key.Y >= yTop)
         && (!yBottom.HasValue || p.Key.Y <= yBottom)));
    }

    public object Clone()
    {
        Grid<T> grid = new Grid<T>();
        foreach (var item in points)
        {
            grid.SetPoint(item.Key, item.Value);
        }
        grid.DefaultValue = DefaultValue;
        grid.MinX = MinX; 
        grid.MaxX = MaxX; 
        grid.MinY = MinY;
        grid.MaxY = MaxY;
        return grid;
    }

    public IEnumerator<KeyValuePair<Point2D, T>> GetEnumerator()
    {
        return points.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

