using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;
public class Graph
{
    public HashSet<Vertex> Vertices;
    public Dictionary<string, Vertex> NamedVertices;
    public Graph()
    {
        Vertices = new();
        NamedVertices = new();
    }
    public int EdgeLength(Vertex a, Vertex b) => a.Edges[b].Length;
    public Vertex AddVertex(Vertex v)
    {
        Vertices.Add(v);
        return v;
    }
    public Vertex AddVertex()
    {
        Vertex v = new();
        return AddVertex(v);
    }
    public Vertex AddNamedVertex(Vertex v, string name)
    {
        NamedVertices.Add(name, v);
        v.Name = name;
        return AddVertex(v);
    }
    public Vertex AddNamedVertex(string name)
    {
        Vertex v = new();
        return AddNamedVertex(v, name);
    }
    public Vertex GetNamedVertex(string name)
    {
        return NamedVertices[name];
    }
    public bool TryGetNamedVertex(string name, out Vertex v)
    {
        if (NamedVertices.TryGetValue(name, out Vertex vert))
        {
            v = vert;
            return true;
        }
        v = null;
        return false;
    }
    public Vertex GetOrCreateNamedVertex(string name)
    {
        if (TryGetNamedVertex(name, out Vertex vert)) return vert;
        return AddNamedVertex(name);
    }

    public void ConnectVertices(Vertex a, Vertex b, int length = 1, bool mutual = true)
    {
        if (mutual) a.ConnectToEachother(b, length);
        else a.ConnectToOneWay(b, length);
    }

    public class Vertex
    {
        public Dictionary<Vertex, Edge> Edges;
        public object? Value;
        public string? Name;
        public Vertex() 
        {
            Edges = new();
        }
        public Vertex(Vertex clone) : this()
        {
            Value = clone.Value;
            Name = clone.Name;
            foreach (var item in clone.Edges)
            {
                Edges.Add(item.Key, item.Value);
            }
        }
        public Edge ConnectToOneWay(Vertex other, int length = 1)
        {
            Edge edge = new(this, other, length);
            Edges[other] = edge;
            return edge;
        }
        public Edge ConnectToEachother(Vertex other, int length = 1)
        {
            Edge e = ConnectToOneWay(other, length);
            other.Edges[this] = e;
            return e;
        }
    }
    //public class Vertex<T> : Vertex
    //{
    //    public T Value;
    //    public Vertex(T val) : base()
    //    {
    //        Value = val;
    //    }
    //}
    public class Edge
    {
        public Vertex A, B;
        public int Length;
        public Edge(Vertex a, Vertex b, int length = 1)
        {
            A = a;
            B = b;
            Length = length;
        }
    }
}