using AOC.Interfaces;
using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AOC.Items.TravellingSalesman;
using static System.Net.Mime.MediaTypeNames;

namespace AOC.Y2023;

internal class D23_2023 : Day<char>
{
    class HikeNode : IPathfinderNode<HikeNode>
    {
        public Point2D Node;
        public int ID;
        public List<HikeNode> connections;
        public List<int> distances;
        public static HashSet<HikeNode> KnownHikeNodes = new();
        public HikeNode(Point2D point)
        {
            Node = point;
            connections = new();
            distances = new();
        }
        public bool ConnectTo(HikeNode node, out int distance)
        {
            distance = connections.IndexOf(node);
            if (distance == -1) return false;
            distance = distances[distance];
            return true;
        }
        public IEnumerable<HikeNode> Adjacent() => connections;
        public bool Equals(HikeNode? other) => Node == other?.Node;
        public bool Equals(HikeNode? x, HikeNode? y) => x?.Equals(y) ?? false;
        public int GetHashCode([DisallowNull] HikeNode obj) => obj.Node.GetHashCode();
        internal void MapConnections(Pathfinder<Point2D, char> pathfinder, bool twoWay = false)
        {
            ID = KnownHikeNodes.Count;
            KnownHikeNodes.Add(this);
            foreach (var path in pathfinder.AllPaths())
            {
                HikeNode? next = KnownHikeNodes.FirstOrDefault(x => x.Node == path.Node);
                //Console.WriteLine($"Mapped {path.Cost} steps from {Node} to {path.Node}");
                if (next is null)
                {
                    next = new HikeNode(path.Node);
                    pathfinder.SetStart(path.Node);
                    next.MapConnections(pathfinder, twoWay);
                }
                connections.Add(next);
                distances.Add(path.Cost);
                if (twoWay)
                {
                    next.connections.Add(this);
                    next.distances.Add(path.Cost);
                }
            }
        }
    }
    public static string arrowChars = "^<>v";
    void MarkHikeNodes(Grid<char> grid)
    {
        foreach (var point in grid.Where(x => x.Key.Adjacent().All(x => grid[x] == '#' || arrowChars.Contains(grid[x]))))
        {
            grid[point.Key] = 'X';
        }
    }
    public override void PartOne()
    {
        //useExampleInput = true;
        Grid<char> grid = new(Input2D, '#');
        Point2D start = grid.GetCorners().TopLeft;
        grid[start] = 'S';
        Point2D end = grid.GetCorners().BottomRight;
        grid[end] = 'E';
        HikeNode startHike = new(start);
        MarkHikeNodes(grid);

        var pathfinder = grid.Pathfinder();
        pathfinder.SetStart(start);
        pathfinder.SetTarget(x => (grid[x.Node] == 'X' || grid[x.Node] == 'E') && x.Cost > 0);
        pathfinder.ConnectivityTest = ConnectivityTest;

        startHike.MapConnections(pathfinder);

        Pathfinder<HikeNode, int> pathfinder2 = new();
        pathfinder2.SetStart(startHike);
        pathfinder2.SetTarget(x => x.Node.Node.Y == 0);
        pathfinder2.CostPenalty = (from, to) => from.Node.ConnectTo(to.Node, out int d) ? d : 999999;
        if (pathfinder2.LongestPath(out var result))
        {
            Submit(result.Cost);
        }
    }
    private bool ConnectivityTest(Pathfinder<Point2D, char>.ActiveNode from, Pathfinder<Point2D, char>.ActiveNode to)
    {
        if (from.Previous?.Node == to.Node) return false; //no doubling back
        if (to.Value == '#') return false; //walls
        if (to.Value == '.') return true; //early exit to improve performance
        if (from.Cost != 0 && from.Value == 'X') return false; //stop on hike nodes
        if (DirectionExt.TryFromChar(to.Value, out Direction d) && d == from.Node.DirectionFrom(to.Node)) return false;
        return true;
    }
    public override void PartTwo()
    {
        Console.ReadLine();
        //useExampleInput = true;
        Grid<char> grid = new(Input2D, '#');
        Point2D start = grid.GetCorners().TopLeft;
        grid[start] = 'S';
        Point2D end = grid.GetCorners().BottomRight;
        grid[end] = 'E';
        HikeNode startHike = new(start);
        MarkHikeNodes(grid);

        var pathfinder = grid.Pathfinder();
        pathfinder.SetStart(start);
        pathfinder.SetTarget(x => (grid[x.Node] == 'X' || grid[x.Node] == 'E') && x.Cost > 0);
        pathfinder.ConnectivityTest = ConnectivityTest;
        
        HikeNode.KnownHikeNodes.Clear();
        startHike.MapConnections(pathfinder, true);

        Pathfinder<HikeNode, int> pathfinder2 = new();
        pathfinder2.SetStart(startHike);
        pathfinder2.SetTarget(x => x.Node.Node.Y == 0);
        pathfinder2.CostPenalty = (from, to) => from.Node.ConnectTo(to.Node, out int d) ? d : 999999;
        //if (pathfinder2.LongestPath(out var result))
        foreach (var result in pathfinder2.AllPaths())
        {
            Console.WriteLine($"found path of length {result.Cost} via:\n\t{result.Node.Node}");
            var previous = result.Previous;
            while (previous is not null)
            {
                Console.WriteLine($"\t{previous.Node.Node}");
                previous= previous.Previous;
            }
            //Submit(result.Cost);
            //Console.ReadLine();
        }
    }
}
