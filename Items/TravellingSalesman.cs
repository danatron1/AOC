using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items;
internal static class TravellingSalesman
{
    static Node startNode = new("DUMMYSTARTNODE");
    static HashSet<Node> locations = new();
    public static void Reset() => locations.Clear();
    public static Node AddNode(string name)
    {
        Node a = locations.FirstOrDefault(n => n.name == name) ?? new(name);
        locations.Add(a);
        Node.AddDistanceBetween(a, startNode, 0);
        return a;
    }
    public static void TryAddConnection(params string[] data)
    {
        if (data.Length != 3) throw new ArgumentException("Requires exactly 3 data points");
        if (data.Where(d => int.TryParse(d, out _)).Count() != 1) throw new ArgumentException($"Couldn't figure out which input was the distance (int) from {data[0]}, {data[1]}, and {data[2]}.");
        for (int i = 0; i < 3; i++)
        {
            if (int.TryParse(data[i], out int dist))
            {
                AddConnection(data[i == 0 ? 1 : 0], data[i == 1 ? 2 : 1], dist);
                return;
            }
        }
        throw new Exception($"Nothing added, reason unknown. Input; {string.Join(", ", data)}");
    }
    public static void AddConnection(string nodeA, string nodeB, int distance, bool addToExistingDistance = false)
    {
        Node a = AddNode(nodeA);
        Node b = AddNode(nodeB);
        Node.AddDistanceBetween(a, b, distance, addToExistingDistance);
    }
    public static IEnumerable<Node[]> GetAllRoutes(bool returnToStart) //brute force
    {
        Node[] routes = StartingRoute(returnToStart);
        foreach (Node[] item in routes.Permutations())
        {
            yield return item;
        }
    }
    public static IEnumerable<int> GetAllRouteLengths(bool returnToStart)
    {
        foreach (Node[] route in GetAllRoutes(returnToStart))
        {
            yield return route.RouteLength();
        }
    }
    private static Node[] StartingRoute(bool returnToStart = true)
    {
        if (!returnToStart) return locations.Append(startNode).ToArray();
        return locations.ToArray();
    }
    public static Node[] GetShortestRoute(bool returnToStart, bool printRoute = false)
    {
        Node.invertLengths = false;
        Node[] solution = ThreeOpt(returnToStart);
        if (printRoute) PrintRoute(solution);
        return solution;
    }
    public static int GetShortest(bool returnToStart, bool printRoute = false)
    {
        return GetShortestRoute(returnToStart, printRoute).RouteLength();
    }
    public static Node[] GetLongestRoute(bool returnToStart, bool printRoute = false)
    {
        Node.invertLengths = true;
        Node[] solution = ThreeOpt(returnToStart);
        Node.invertLengths = false;
        if (printRoute) PrintRoute(solution);
        return solution;
    }
    public static int GetLongest(bool returnToStart, bool printRoute = false)
    {
        return GetLongestRoute(returnToStart, printRoute).RouteLength();
    }
    public static void PrintRoute(Node[] route)
    {
        int sum = 0;
        sum += route[^1].To(route[0]);
        route[^1].PrintTo(route[0], sum);
        for (int i = 0; i < route.Length - 1; i++)
        {
            sum += route[i].To(route[i+1]);
            route[i].PrintTo(route[i + 1], sum);
        }
    }
    public static int RouteLength(this Node[] route)
    {
        return route.Append(route[0]).SelectWithPrevious((prev, curr) => prev.To(curr)).Sum();
    }
    public static Node[] ThreeOpt(bool returnToStart = true)
    {
        Node[] route = StartingRoute(returnToStart);
        for (int i = 0; i < route.Length; i++)
        {
            int delta = 0;
            foreach ((int a, int b, int c) item in AllSegments(route.Length))
            {
                delta += ReverseSegmentIfBetter(item.a, item.b, item.c);
            }
            if (delta < 0) i = 0;
        }
        return route;

        int ReverseSegmentIfBetter(int i, int j, int k)
        {
            //algorithm from:
            //https://en.wikipedia.org/wiki/3-opt
            Node A = route[(i + route.Length - 1) % route.Length]; //wrap around
            Node B = route[i];
            Node C = route[j - 1];
            Node D = route[j];
            Node E = route[k - 1];
            Node F = route[k % route.Length];
            int d0 = A.To(B) + C.To(D) + E.To(F);
            int d1 = A.To(C) + B.To(D) + E.To(F);
            int d2 = A.To(B) + C.To(E) + D.To(F);
            int d3 = A.To(D) + E.To(B) + C.To(F);
            int d4 = F.To(B) + C.To(D) + E.To(A);
            if (d0 > d1)
            {
                route.Reverse(i..j);
                return d1 - d0;
            }
            if (d0 > d2)
            {
                route.Reverse(j..k);
                return d2 - d0;
            }
            if (d0 > d4)
            {
                route.Reverse(i..k);
                return d4 - d0;
            }
            if (d0 > d3)
            {
                //Console.WriteLine("\nBEFORE");
                //PrintRoute(route);
                route.Reverse(i..j);
                route.Reverse(j..k); //it's complicated
                route.Reverse(i..k);
                //Console.WriteLine($"{d0} = {A.To(B)} ({A} to {B}) + {C.To(D)} ({C} to {D}) + {E.To(F)} ({E} to {F})");
                //Console.WriteLine($"{d3} = {A.To(D)} ({A} to {D}) + {E.To(B)} ({E} to {B}) + {C.To(F)} ({C} to {F})");
                //Console.WriteLine($"({i},{j},{k}) Found better length: {route.RouteLength()}");
                //Console.WriteLine("AFTER");
                //PrintRoute(route);
                return d3 - d0;
            }
            return 0;
        }
        IEnumerable<(int, int, int)> AllSegments(int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 2; j < n; j++)
                {
                    for (int k = j + 2; k < n + (i > 0 ? 1 : 0); k++)
                    {
                        yield return (i, j, k);
                    }
                }
            }
        }
    }

    public class Node
    {
        public static bool invertLengths = false;
        public string name;
        Dictionary<Node, int> distances;
        public Node() : this(locations.Count.ToString()) { }
        public int To(Node b)
        {
            if (this == startNode) return 0;
            return invertLengths ? -distances[b] : distances[b];
        }
        public void PrintTo(Node b, int rollingTotal = 0)
        {
            Console.WriteLine($"{name} to {b.name} = {To(b)}{(rollingTotal == 0 ? "" : $" (total {rollingTotal})")}");
        }
        public override string ToString() => name;
        public Node(string n)
        {
            name = n;
            distances = new Dictionary<Node, int>();
        }
        internal static void AddDistanceBetween(Node a, Node b, int distance, bool addToExistingDistance = false)
        {
            if (!a.distances.ContainsKey(b)) a.distances.Add(b, distance);
            else if (addToExistingDistance) a.distances[b] += distance;
            if (!b.distances.ContainsKey(a)) b.distances.Add(a, distance);
            else if (addToExistingDistance) b.distances[a] += distance;

        }
    }
}
