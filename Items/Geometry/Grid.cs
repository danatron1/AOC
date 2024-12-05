using System.Collections;
using System.Data;

namespace AOC.Items.Geometry;
public class Grid<T> : ICloneable, IEnumerable<KeyValuePair<Point2D, T>> where T : notnull
{
    internal Dictionary<Point2D, T> points = new();
    public Dictionary<T, string> printedRepresentation = new();
    public int? MaxX, MaxY, MinX, MinY;
    public T DefaultValue = default!;
    public int Count => points.Count;

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
        DefaultValue = defaultValue;
    }
    public Grid(T[,] input2d)
    {
        SetPoints(input2d);
    }
    public Grid(T value, IEnumerable<Point2D> input2d)
    {
        SetPoints(value, input2d);
    }


    public Grid(T[,] input2d, T defaultValue) : this(defaultValue)
    {
        SetPoints(input2d);
    }
    public Pathfinder<Point2D, T> Pathfinder()
    {
        Pathfinder<Point2D, T> pf = new();
        pf.Map = (p) => this[p];
        pf.DistanceMap = (f, t) => f.ManhattanDistanceTo(t);
        if (this is Grid<int> intGrid) pf.CostPenalty = (_, n) => intGrid[n.Node];
        return pf;
    }
    public GridPathfinder<T> CreatePathfinder() => new(this);
    public IEnumerable<KeyValuePair<Point2D, T>> ValuesInColumn(Point2D of) => ValuesInColumn(of.X);
    public IEnumerable<KeyValuePair<Point2D, T>> ValuesInColumn(int column)
    {
        foreach (var item in points.Where(p => p.Key.X == column)) yield return item;
    }
    public IEnumerable<KeyValuePair<Point2D, T>> ValuesInRow(Point2D of) => ValuesInRow(of.Y);
    public IEnumerable<KeyValuePair<Point2D, T>> ValuesInRow(int row)
    {
        foreach (var item in points.Where(p => p.Key.Y == row)) yield return item;
    }
    public IEnumerable<Point2D> PointsInColumn(Point2D of) => PointsInColumn(of.X);
    public IEnumerable<Point2D> PointsInColumn(int column)
    {
        var limits = GetLimitsOrExtremes();
        return PointsInArea(column, limits.maxY, column, limits.minY);
    }
    public IEnumerable<Point2D> PointsInRow(Point2D of) => PointsInRow(of.Y);
    public IEnumerable<Point2D> PointsInRow(int row)
    {
        var limits = GetLimitsOrExtremes();
        return PointsInArea(limits.maxX, row, limits.minX, row);
    }
    public static IEnumerable<Point2D> PointsInArea((int maxX, int maxY, int minX, int minY) area) => PointsInArea(area.maxX, area.maxY, area.minX, area.minY);
    public static IEnumerable<Point2D> PointsInArea(Point2D a, Point2D b)
    {
        Point2D mins = (Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        Point2D maxs = (Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        return PointsInArea(maxs.X, maxs.Y, mins.X, mins.Y);
    }
    public static IEnumerable<Point2D> PointsInArea(int maxX, int maxY, int minX, int minY)
    {
        for (int y = maxY; y >= minY; y--)
        {
            for (int x = minX; x <= maxX; x++)
            {
                yield return (x, y);
            }
        }
    }
    public IEnumerable<int> Columns()
    {
        var limits = GetLimitsOrExtremes();
        for (int x = limits.minX; x <= limits.maxX; x++) yield return x;
    }
    public IEnumerable<int> Rows()
    {
        var limits = GetLimitsOrExtremes();
        for (int y = limits.maxY; y >= limits.minY; y--) yield return y;
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
        if (p.X > MaxX || p.X < MinX || p.Y > MaxY || p.Y < MinY) return;
        if (points.ContainsKey(p))
        {
            if (value is null || value.Equals(DefaultValue)) points.Remove(p);
            else points[p] = value;
        }
        else if (!(value is null || value.Equals(DefaultValue))) points.Add(p, value);
        GridModified();
    }
    public Point2D? FindPoint(T value)
    {
        return GetPointsAndValues().FirstOrDefault(x => x.value.Equals(value)).point;
    }
    public T GetPoint(Point2D p)
    {
        if (points.TryGetValue(p, out T v)) return v;
        else return DefaultValue;
    }
    public bool ContainsPoint(Point2D p) => points.ContainsKey(p);
    public IEnumerable<Point2D> GetPoints(Func<KeyValuePair<Point2D, T>, bool> predicate)
    {
        return points.Where(predicate).Select(s => s.Key);
    }
    public IEnumerable<Point2D> GetPoints() => points.Select(s => s.Key);
    public IEnumerable<T> GetValues(Func<KeyValuePair<Point2D, T>, bool> predicate)
    {
        return points.Where(predicate).Select(s => s.Value);
    }
    public IEnumerable<T> GetValues() => points.Select(s => s.Value);
    public IEnumerable<(Point2D point, T value)> GetPointsAndValues(Func<KeyValuePair<Point2D, T>, bool> predicate)
    {
        return points.Where(predicate).Select(s => (s.Key, s.Value));
    }
    public IEnumerable<(Point2D point, T value)> GetPointsAndValues() => points.Select(s => (s.Key, s.Value));
    public T[,] To2DArray()
    {
        var limits = GetLimitsOrExtremes();
        T[,] array = new T[1 + limits.maxX - limits.minX, 1 + limits.maxY - limits.minY];
        foreach (Point2D item in GetPoints())
        {
            array[item.X - limits.minX, limits.maxY - item.Y] = GetPoint(item);
        }
        return array;
    }
    public void SetPoints(T value, IEnumerable<Point2D> input2d) => SetPoints(value, input2d.ToArray());
    public void SetPoints(T value, params Point2D[] points)
    {
        foreach (Point2D point in points)
        {
            SetPoint(point, value);
        }
    }
    public void SetPoints(T[,] input)
    {
        //made consistent so that bottom left = 0, 0
        //bottom right = x, 0
        //top left = 0, x
        //top right = x, x
        Point2D max = (input.GetLength(0), input.GetLength(1));
        for (int y = 0; y < max.Y; y++)
        {
            for (int x = 0; x < max.X; x++)
            {
                SetPoint((x, max.Y - 1 - y), input[x, y]);
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
        string line;
        for (int y = limits.maxY; y >= limits.minY; y--)
        {
            line = "";
            for (int x = limits.minX; x <= limits.maxX; x++)
            {
                T? point = GetPoint((x, y));
                if (point is null) line += "NULL";
                else if (printedRepresentation.TryGetValue(point, out string? rep)) line += rep;
                else line += point;
            }
            Console.WriteLine(line);
        }
    }
    /// <summary>
    /// reads through the points in the grid in order.<br/>
    /// Order is the same as English text: left to right, top to bottom.
    /// </summary>
    /// <returns>yields the grid points in sequence.</returns>
    public IEnumerable<Point2D> Scan(bool includeEmptyPoints = true)
    {
        var limits = GetLimitsOrExtremes();
        for (int y = limits.maxY; y >= limits.minY; y--)
        {
            for (int x = limits.minX; x <= limits.maxX; x++)
            {
                Point2D p = (x, y);
                if (includeEmptyPoints || points.ContainsKey(p)) yield return p;
            }
        }
    }
    void GridModified()
    {
        _cachedLimits = null;
    }
    private (int maxX, int maxY, int minX, int minY)? _cachedLimits;
    public (int maxX, int maxY, int minX, int minY) GetLimitsOrExtremes()
    {
        if (_cachedLimits is null)
        {
            (int maxX, int maxY, int minX, int minY) output;
            output.maxX = MaxX ?? points.MaxBy(x => x.Key.X).Key.X;
            output.maxY = MaxY ?? points.MaxBy(y => y.Key.Y).Key.Y;
            output.minX = MinX ?? points.MinBy(x => x.Key.X).Key.X;
            output.minY = MinY ?? points.MinBy(y => y.Key.Y).Key.Y;
            _cachedLimits = output;
        }
        return _cachedLimits.Value;
    }
    internal (Point2D TopLeft, Point2D TopRight, Point2D BottomLeft, Point2D BottomRight) GetCorners()
    {
        var limits = GetLimitsOrExtremes();
        (Point2D TopLeft, Point2D TopRight, Point2D BottomLeft, Point2D BottomRight) output;
        output.TopLeft = (limits.minX, limits.maxY);
        output.TopRight = (limits.maxX, limits.maxY);
        output.BottomLeft = (limits.minX, limits.minY);
        output.BottomRight = (limits.maxX, limits.minY);
        return output;
    }
    internal Grid<T> PointsFrom(Point2D point, Direction direction, bool lineOnly)
    {
        return PointsFrom(point.X, point.Y, direction, lineOnly);
    }
    internal Grid<T> PointsFrom(int x, int y, Direction direction, bool lineOnly)
    {
        return direction switch
        {
            Direction.North => lineOnly ? Subgrid(x, x, null, y - 1) : Subgrid(null, null, null, y - 1),
            Direction.East => lineOnly ? Subgrid(x + 1, null, y, y) : Subgrid(x + 1, null, null, null),
            Direction.South => lineOnly ? Subgrid(x, x, y + 1, null) : Subgrid(null, null, y + 1, null),
            Direction.West => lineOnly ? Subgrid(null, x - 1, y, y) : Subgrid(null, x - 1, null, null),
            _ => new Grid<T>(),
        };
    }
    internal bool PointInBounds(Point2D point)
    {
        var limits = GetLimitsOrExtremes();
        return point.X >= limits.minX && point.Y >= limits.minY
            && point.X <= limits.maxX && point.Y <= limits.maxY;
    }
    internal IEnumerable<Point2D> WalkWhile(Point2D start, Direction direction, Func<Point2D, bool> predicate)
    {
        while (true)
        {
            start = start.NextIn(direction);
            if (!predicate(start)) break;
            yield return start;
        }
    }
    internal IEnumerable<Point2D> WalkToEdge(Point2D start, Direction direction)
        => WalkWhile(start, direction, PointInBounds);
    internal IEnumerable<Point2D> WalkForever(Point2D start, Direction direction)
        => WalkWhile(start, direction, _ => true);
    internal Grid<T> Subgrid(int? xLeft, int? xRight, int? yTop, int? yBottom)
    {
        return new Grid<T>(points.Where(p =>
            (!xLeft.HasValue || p.Key.X >= xLeft)
         && (!xRight.HasValue || p.Key.X <= xRight)
         && (!yTop.HasValue || p.Key.Y >= yTop)
         && (!yBottom.HasValue || p.Key.Y <= yBottom)));
    }
    public object Clone() => CloneGrid();
    public Grid<T> CloneGrid()
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
    public Graph ToGraph() => ToGraph(_ => true);
    public Graph ToGraph(Func<(T from, T to), bool> connectionRule)
    {
        Graph graph = new Graph();
        foreach (var item in points)
        {
            Graph.Vertex vert = graph.GetOrCreateNamedVertex(item.Key.ToString());
            vert.Value = item.Value;
            foreach (Point2D adj in item.Key.Adjacent())
            {
                if (points.TryGetValue(adj, out T value)) //if adjacent node exists
                {
                    Graph.Vertex v = graph.GetOrCreateNamedVertex(adj.ToString());
                    v.Value = value;
                    if (vert.Value is T t1 && v.Value is T t2 && connectionRule((t1, t2)))
                    {
                        graph.ConnectVertices(vert, v, mutual: false);
                    }
                }
            }
        }
        return graph;
    }

    internal void Clear()
    {
        points.Clear();
        GridModified();
    }

    internal void AddBorder(T borderChar)
    {
        var corner = GetCorners();
        FillArea(corner.TopLeft.North, corner.TopRight.North.East, borderChar);
        FillArea(corner.TopRight.East, corner.BottomRight.East.South, borderChar);
        FillArea(corner.TopLeft.West.North, corner.BottomLeft.West, borderChar);
        FillArea(corner.BottomLeft.West.South, corner.BottomRight.South, borderChar);
    }
    internal void FillArea(Point2D topLeft, Point2D bottomRight, T fill)
    {
        for (int i = topLeft.Y; i >= bottomRight.Y; i--)
        {
            for (int j = topLeft.X; j <= bottomRight.X; j++)
            {
                SetPoint((j, i), fill);
            }
        }
    }
}

