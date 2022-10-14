using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;
public class Graph
{
    public Vertex[] Vertices;
    public int EdgeLength(Vertex a, Vertex b) => a.Edges[b].Length;

    public class Vertex
    {
        public Dictionary<Vertex, Edge> Edges;
        public Vertex(Vertex node)
        {
            Edges = new();
            foreach (var item in node.Edges)
            {
                Edges.Add(item.Key, item.Value);
            }
        }
    }
    public class Edge
    {
        public Vertex A, B;
        public int Length;
    }
}
