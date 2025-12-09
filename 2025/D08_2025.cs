using AOC.Items.Geometry;
using AOC.Items.Graphs;

namespace AOC.Y2025;

internal class D08_2025 : Day
{
    public override void PartOne()
    {
        HashSet<PointVertex<Point3D>> junctions = Input.Select(x => new PointVertex<Point3D>(new Point3D(x))).ToHashSet();

        Edge[] edges = junctions.Pairs().Select(x => new Edge(x[0], x[1], x[0].Point.EuclideanDistanceTo(x[1].Point))).OrderBy(x => x.Length).ToArray();

        for (int i = 0; i < (useExampleInput ? 10 : 1000); i++)
        {
            edges[i].A.AddEdge(edges[i].B);
        }

        List<HashSet<Vertex>> circuits = new();
        foreach (PointVertex<Point3D> vertex in junctions)
        {
            if (vertex.Edges.Count == 0) continue;
            if (circuits.Any(x => x.Contains(vertex))) continue;
            circuits.Add(vertex.ExploreConnectedVertices().ToHashSet());
        }

        circuits = circuits.OrderByDescending(x => x.Count).Take(3).ToList();

        Submit(circuits.Select(x => x.Count).Mul());
    }
    public override void PartTwo()
    {
        HashSet<PointVertex<Point3D>> junctions = Input.Select(x => new PointVertex<Point3D>(new Point3D(x))).ToHashSet();
        var edges = junctions.Pairs().Select(x => new Edge(x[0], x[1], x[0].Point.EuclideanDistanceTo(x[1].Point))).OrderBy(x => x.Length);
        Edge lastEdge = null;

        foreach (Edge edge in edges)
        {
            if (edge.A.Graph.Vertices.Contains(edge.B)) continue;
            lastEdge = edge.A.AddEdge(edge.B, edge.Length);
            if (junctions.First().Graph.Count == junctions.Count) break;
        }

        if (lastEdge is not null && lastEdge.A is PointVertex<Point3D> a && lastEdge.B is PointVertex<Point3D> b)
        {
            Submit((long)a.Point.X * b.Point.X);
        }
    }
}