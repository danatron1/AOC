using AOC.Interfaces;
using AOC.Items.Geometry;

namespace AOC.Items.Graphs;   

public class Vertex
{
    public Graph Graph { get; set; }

    private HashSet<Edge> _edges;
    public HashSet<Edge> Edges
    {
        get { return _edges; }
    }

    public Vertex() : this(new Graph()) { }
    public Vertex(Graph graph)
    {
        _edges = new();
        Graph = graph;
        Graph.AddVertex(this);
    }

    public Edge AddEdge(Vertex other, double? length = null)
    {
        Edge edge = new(this, other, length);
        Edges.Add(edge);
        if (Graph != other.Graph)
        {
            Graph.UnionWith(other.Graph);
        }
        other.Edges.Add(edge);
        return edge;
    }
    public IEnumerable<Vertex> ExploreConnectedVertices() => ExploreConnectedVertices(new());
    private IEnumerable<Vertex> ExploreConnectedVertices(HashSet<Vertex> ignore)
    {
        if (ignore.Contains(this)) yield break;
        yield return this;
        ignore.Add(this);
        foreach (Edge edge in Edges)
        {
            if (edge.A != this) foreach (Vertex v in edge.A.ExploreConnectedVertices(ignore)) yield return v;
            if (edge.B != this) foreach (Vertex v in edge.B.ExploreConnectedVertices(ignore)) yield return v;
        }
    }
    public IEnumerable<Edge> ExploreConnectedEdges() => ExploreConnectedEdges(new());
    private IEnumerable<Edge> ExploreConnectedEdges(HashSet<Edge> ignore)
    {
        foreach (Vertex vertex in ExploreConnectedVertices())
        {
            foreach (Edge edge in vertex.Edges)
            {
                if (ignore.Contains(edge)) continue;
                yield return edge;
                ignore.Add(edge);
            }
        }

    }
}

public class Vertex<T> : Vertex //vertex with data
{
    public T? Value;
    public Vertex() : this(default) { }
    public Vertex(T? value) : base()
    {
        Value = value;
    }
    public override string? ToString() => Value?.ToString();
}

public class PointVertex<T> : Vertex, IPositional<T> where T : IPoint<T>
{
    public T Point { get; set; }
    public PointVertex() : this(T.Zero) { }
    public PointVertex(T point) : base()
    {
        Point = point;
    }
    public override string? ToString() => Point.ToString();
    public int ManhattanDistanceTo(IPositional<T> other) => Point.ManhattanDistanceTo(other.Point);
    public double EuclideanDistanceTo(IPositional<T> other) => Point.EuclideanDistanceTo(other.Point);
}
