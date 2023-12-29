using AOC.Interfaces;
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

namespace AOC.Items.Geometry;
public class Grid3D<T> : ICloneable, IEnumerable<KeyValuePair<Point3D, T>> where T : notnull
{
    internal Dictionary<Point3D, T> points = new();
    public Dictionary<T, string> printedRepresentation = new();
    public int? MaxX, MaxY, MinX, MinY, MaxZ, MinZ;
    public T DefaultValue = default!;
    public int Count => points.Count;

    public Grid3D() { }
    public Grid3D(IEnumerable<KeyValuePair<Point3D, T>> grid)
    {
        foreach (var item in grid)
        {
            points.Add(item.Key, item.Value);
        }
    }
    public Grid3D(T defaultValue)
    {
        DefaultValue = defaultValue;
    }
    public Grid3D(T[,,] input2d)
    {
        SetPoints(input2d);
    }
    public Grid3D(T value, IEnumerable<Point3D> input3d)
    {
        SetPoints(value, input3d);
    }


    public Grid3D(T[,,] input3d, T defaultValue) : this(defaultValue)
    {
        SetPoints(input3d);
    }
    public Pathfinder<Point3D, T> Pathfinder()
    {
        Pathfinder<Point3D, T> pf = new();
        pf.Map = (p) => this[p];
        pf.DistanceMap = (f, t) => f.ManhattanDistanceTo(t);
        if (this is Grid3D<int> intGrid) pf.CostPenalty = (_, n) => intGrid[n.Node];
        return pf;
    }
    public static IEnumerable<Point3D> PointsInArea((int maxX, int maxY, int maxZ, int minX, int minY, int minZ) area) => PointsInArea(area.maxX, area.maxY, area.maxZ, area.minX, area.minY, area.minZ);
    public static IEnumerable<Point3D> PointsInArea(Point3D a, Point3D b)
    {
        Point3D mins = (Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        Point3D maxs = (Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        return PointsInArea(maxs.X, maxs.Y, maxs.Z, mins.X, mins.Y, mins.Z);
    }
    public static IEnumerable<Point3D> PointsInArea(int maxX, int maxY, int maxZ, int minX, int minY, int minZ)
    {
        for (int z = maxZ; z >= minZ; z--)
        {
            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    yield return (x, y, z);
                }
            }
        }
    }
    public void SetLimits(int? maxX = null, int? maxY = null, int? maxZ = null, 
                          int? minX = 0, int? minY = 0, int? minZ = 0)
    {
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;
        MinX = minX;
        MinY = minY;
        MinZ = minZ;
    }
    public void SetPoint(Point3D p, T value)
    {
        if (p.X > MaxX || p.X < MinX || p.Y > MaxY || p.Y < MinY || p.Z > MaxX || p.Z < MinZ) return;
        if (points.ContainsKey(p))
        {
            if (value is null || value.Equals(DefaultValue)) points.Remove(p);
            else points[p] = value;
        }
        else if (!(value is null || value.Equals(DefaultValue))) points.Add(p, value);
        GridModified();
    }
    public Point3D? FindPoint(T value)
    {
        return GetPointsAndValues().FirstOrDefault(x => x.value.Equals(value)).point;
    }
    public T GetValue(Point3D p)
    {
        if (points.TryGetValue(p, out T v)) return v;
        else return DefaultValue;
    }
    public bool ContainsPoint(Point3D p) => points.ContainsKey(p);
    public IEnumerable<Point3D> GetPoints(Func<KeyValuePair<Point3D, T>, bool> predicate)
    {
        return points.Where(predicate).Select(s => s.Key);
    }
    public IEnumerable<Point3D> GetPoints() => points.Select(s => s.Key);
    public IEnumerable<T> GetValues(Func<KeyValuePair<Point3D, T>, bool> predicate)
    {
        return points.Where(predicate).Select(s => s.Value);
    }
    public IEnumerable<T> GetValues() => points.Select(s => s.Value);
    public IEnumerable<(Point3D point, T value)> GetPointsAndValues(Func<KeyValuePair<Point3D, T>, bool> predicate)
    {
        return points.Where(predicate).Select(s => (s.Key, s.Value));
    }
    public IEnumerable<(Point3D point, T value)> GetPointsAndValues() => points.Select(s => (s.Key, s.Value));
    public T[,,] To3DArray()
    {
        var limits = GetLimitsOrExtremes();
        T[,,] array = new T[1 + limits.maxX - limits.minX, 1 + limits.maxY - limits.minY, 1+limits.maxZ - limits.minZ];
        foreach (Point3D item in GetPoints())
        {
            array[item.X - limits.minX, limits.maxY - item.Y, item.Z - limits.minZ] = GetValue(item);
        }
        return array;
    }
    public void SetPoints(T value, IEnumerable<Point3D> input3d) => SetPoints(value, input3d.ToArray());
    public void SetPoints(T value, params Point3D[] points)
    {
        foreach (Point3D point in points)
        {
            SetPoint(point, value);
        }
    }
    public void SetPoints(T[,,] input)
    {
        //made consistent so that;
        //bottom left = 0, 0
        //bottom right = x, 0
        //top left = 0, x
        //top right = x, x
        Point3D rows = (input.GetLength(0), input.GetLength(1), input.GetLength(2));
        for (int z = 0; z < rows.Z; z++)
        {
            for (int y = 0; y < rows.Y; y++)
            {
                for (int x = 0; x < rows.X; x++)
                {
                    SetPoint((x, rows.Y - 1 - y, rows.Z - 1 - z), input[x, y, z]);
                }
            }
        }
    }
    public T this[Point3D p]
    {
        get => GetValue(p);
        set => SetPoint(p, value);
    }
    public T this[int x, int y, int z]
    {
        get => this[(x, y, z)];
        set => this[(x, y, z)] = value;
    }
    public void PrintBoard(int? zSlice = null)
    {
        var limits = GetLimitsOrExtremes();
        string line;
        for (int y = limits.maxY; y >= limits.minY; y--)
        {
            line = "";
            for (int x = limits.minX; x <= limits.maxX; x++)
            {
                T? point = DefaultValue;
                int z = zSlice ?? limits.maxZ;
                while (point.Equals(DefaultValue))
                {
                    point = GetValue((x, y, z));
                    if (zSlice.HasValue || z-- == limits.minZ) break;
                }
                if (printedRepresentation.TryGetValue(point, out string? rep)) line += rep;
                else line += point;
            }
            Console.WriteLine(line);
        }
    }
    /// <summary>
    /// reads through the points in the grid in order.<br/>
    /// Order is the same as English books: left to right, top to bottom, front to back.
    /// </summary>
    /// <returns>yields the grid points in sequence.</returns>
    public IEnumerable<Point3D> Scan(bool includeEmptyPoints = true)
    {
        var lim = GetLimitsOrExtremes();
        foreach (Point3D point in PointsInArea(lim.maxX, lim.maxY, lim.maxZ, lim.minX, lim.minY, lim.minZ))
        {
            if (includeEmptyPoints || points.ContainsKey(point)) yield return point;
        }
    }
    void GridModified()
    {
        _cachedLimits = null;
    }
    private (int maxX, int maxY, int maxZ, int minX, int minY, int minZ)? _cachedLimits;
    public (int maxX, int maxY, int maxZ, int minX, int minY, int minZ) GetLimitsOrExtremes()
    {
        if (_cachedLimits is null)
        {
            (int maxX, int maxY, int maxZ, int minX, int minY, int minZ) output;
            output.maxX = MaxX ?? points.MaxBy(x => x.Key.X).Key.X;
            output.maxY = MaxY ?? points.MaxBy(y => y.Key.Y).Key.Y;
            output.maxZ = MaxZ ?? points.MaxBy(z => z.Key.Z).Key.Z;
            output.minX = MinX ?? points.MinBy(x => x.Key.X).Key.X;
            output.minY = MinY ?? points.MinBy(y => y.Key.Y).Key.Y;
            output.minZ = MinZ ?? points.MinBy(z => z.Key.Z).Key.Z;
            _cachedLimits = output;
        }
        return _cachedLimits.Value;
    }
    internal (Point3D TopLeftFront, Point3D TopLeftBack, Point3D TopRightFront, Point3D TopRightBack, Point3D BottomLeftFront, Point3D BottomLeftBack, Point3D BottomRightFront, Point3D BottomRightBack) 
        GetCorners()
    {
        var limits = GetLimitsOrExtremes();
        (Point3D TopLeftFront, Point3D TopLeftBack,
        Point3D TopRightFront, Point3D TopRightBack,
        Point3D BottomLeftFront, Point3D BottomLeftBack,
        Point3D BottomRightFront, Point3D BottomRightBack) output;
        output.TopLeftFront = (limits.minX, limits.maxY, limits.maxZ);
        output.TopRightFront = (limits.maxX, limits.maxY, limits.maxZ);
        output.BottomLeftFront = (limits.minX, limits.minY, limits.maxZ);
        output.BottomRightFront = (limits.maxX, limits.minY, limits.maxZ);
        output.TopLeftBack = (limits.minX, limits.maxY, limits.minZ);
        output.TopRightBack = (limits.maxX, limits.maxY, limits.minZ);
        output.BottomLeftBack = (limits.minX, limits.minY, limits.minZ);
        output.BottomRightBack = (limits.maxX, limits.minY, limits.minZ);
        return output;
    }
    internal bool PointInBounds(Point3D point)
    {
        var limits = GetLimitsOrExtremes();
        return point.X >= limits.minX && point.Y >= limits.minY && point.Z >= limits.minZ
            && point.X <= limits.maxX && point.Y <= limits.maxY && point.Z <= limits.maxZ;
    }
    internal IEnumerable<Point3D> WalkWhile(Point3D start, Direction3D direction, Func<Point3D, bool> predicate)
    {
        while (true)
        {
            start = start.NextIn(direction);
            if (!predicate(start)) break;
            yield return start;
        }
    }
    internal IEnumerable<Point3D> WalkToEdge(Point3D start, Direction3D direction)
        => WalkWhile(start, direction, PointInBounds);
    internal IEnumerable<Point3D> WalkForever(Point3D start, Direction3D direction)
        => WalkWhile(start, direction, _ => true);
    internal Grid3D<T> Subgrid(int? xLeft, int? xRight, int? yTop, int? yBottom, int? zFront, int? zBack)
    {
        return new Grid3D<T>(points.Where(p =>
            (!xLeft.HasValue || p.Key.X >= xLeft)
         && (!xRight.HasValue || p.Key.X <= xRight)
         && (!yTop.HasValue || p.Key.Y >= yTop)
         && (!yBottom.HasValue || p.Key.Y <= yBottom)
         && (!zFront.HasValue || p.Key.Z >= zFront)
         && (!zBack.HasValue || p.Key.Z <= zBack)));
    }
    public object Clone() => CloneGrid();
    public Grid3D<T> CloneGrid()
    {
        Grid3D<T> grid = new Grid3D<T>();
        foreach (var item in points)
        {
            grid.SetPoint(item.Key, item.Value);
        }
        grid.DefaultValue = DefaultValue;
        grid.MinX = MinX;
        grid.MaxX = MaxX;
        grid.MinY = MinY;
        grid.MaxY = MaxY;
        grid.MinZ = MinZ;
        grid.MaxZ = MaxZ;
        return grid;
    }

    public IEnumerator<KeyValuePair<Point3D, T>> GetEnumerator()
    {
        return points.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    internal void Clear()
    {
        points.Clear();
        GridModified();
    }
    internal void FillArea(Point3D cornerA, Point3D cornerB, T fill)
    {
        foreach (Point3D point in PointsInArea(cornerA, cornerB))
        {
            SetPoint(point, fill);
        }
    }
}

