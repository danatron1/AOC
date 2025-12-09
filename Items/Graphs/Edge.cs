namespace AOC.Items.Graphs;

public class Edge
{
    public Vertex A, B;
    public double? Length;
    public Edge(Vertex a, Vertex b, double? length = null)
    {
        A = a;
        B = b;
        Length = length;
    }
    public override string ToString() => $"{A} {Length}-> {B}";
    public void Remove()
    {
        A.Edges.Remove(this);
        B.Edges.Remove(this);
    }
}
