namespace AOC.Items.Graphs;

public class Graph
{
    public HashSet<Vertex> Vertices { get; set; }
    public int Count => Vertices.Count;
    public Graph(HashSet<Vertex> vertices)
    {
        Vertices = vertices;
    }
    public Graph() : this(new HashSet<Vertex>()) { }
    public Graph(params Vertex[] vertices) : this(vertices.ToHashSet()) { }
    public void AddVertex(Vertex vertex) => Vertices.Add(vertex);
    internal void UnionWith(Graph graph)
    {
        Vertices.UnionWith(graph.Vertices);
        foreach (Vertex vertex in Vertices) vertex.Graph = this;
    }
}