using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AOC.Items.Graph;

namespace AOC.Items;
public class Pathfinder
{
    #region UNFINISHED PROJECT
    //Vertex Start;
    Graph Graph;

    bool ReachVertex(Vertex target, Vertex start, out ReachedVertex found)
    {
        List<ReachedVertex> reached = new();
        PriorityQueue<ReachedVertex, int> queue = new();
        found = new ReachedVertex(start, null, 0);
        foreach (Vertex node in Graph.Vertices)
        {
            reached.Add(new(node, null, int.MaxValue));
            queue.Enqueue(reached[^1], int.MaxValue);
        }
        while (queue.Count > 0)
        {
            found = queue.Dequeue();
            if (found == target) return true;
            foreach (KeyValuePair<Vertex, Edge> neighbour in found.Edges)
            {
                int dist = found.Distance + found.Edges[neighbour.Key].Length;
                if (reached.FirstOrDefault(x => x == neighbour.Key) is ReachedVertex known)
                {
                    if (dist < known.Distance)
                    {
                        known.Distance = dist;
                        known.Previous = found;
                    }
                }
                else reached.Add(new(neighbour.Key, found, dist));
            }
        }
        return false;
    }
    public class ReachedVertex : Vertex
    {
        public bool Start;
        public Vertex? Previous;
        public int Distance;
        public ReachedVertex(Vertex node, Vertex? previous, int distance) : base(node)
        {
            Start = previous == null;
            Previous = previous;
            Distance = distance;
        }
    }
    #endregion
}
