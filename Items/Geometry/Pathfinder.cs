using AOC.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static AOC.Items.TravellingSalesman;

namespace AOC.Items.Geometry
{
    public class Pathfinder<TNode, TMap> where TNode : notnull, IPathfinderNode<TNode>
    {
        public class ActiveNode
        {
            public TNode Node;
            public int Cost = 0;
            public int Distance = 0;
            public string Memory = string.Empty; //used to store arbitrary information for use with ConnectivityRule
            public int CostDistance => Cost + Distance;
            public TMap Value;
            public ActiveNode? Previous;
            public ActiveNode(TNode node, TMap value)
            {
                Node = node;
                Value = value;
            }
            internal void AssignPrevious(Pathfinder<TNode, TMap>.ActiveNode from,
                Pathfinder<TNode, TMap>.CostPenaltyDelegate costPenalty,
                Pathfinder<TNode, TMap>.MemoryDelegate nodeMemoryMap,
                Func<TNode, TNode, int> distanceMap,
                TNode? target)
            {
                Previous = from;
                Memory = nodeMemoryMap(from, this);
                if (target is not null) Distance = distanceMap(Node, target);
                Cost = from.Cost + costPenalty(from, this);
            }
        }
        public TNode? Target { get; private set; } = default;
        private Func<ActiveNode, bool>? VictoryTest;

        private IEnumerable<TNode>? Starts;

        /// <summary>
        /// Takes a <typeparamref name="TNode"/> input and returns a <typeparamref name="TMap"/> <br/>
        /// Used to turn a node (e.g. a position) into a value (e.g. wall or floor) <br/>
        /// <example>
        /// E.g. Typically used to look up values as an index: <c>p => grid[p]</c>
        /// </example>
        /// </summary>
        public Func<TNode, TMap> Map = n => default;

        /// <summary>
        /// Takes two <typeparamref name="TNode"/> inputs (START and TARGET) and outputs an int <br/>
        /// Used to determine which nodes are physically nearer to the target. <br/>
        /// Optional, however pathfinding performance is improved if distance is included. <br/>
        /// <example>
        /// E.g. Can be specified to use a particular distance calculation: <br/> 
        /// <c>(start, target) => start.ManhattanDistanceTo(target)</c> <br/>
        /// <c>(start, target) => start.PythagorasDistanceTo(target)</c>
        /// </example>
        /// </summary>
        public Func<TNode, TNode, int> DistanceMap = (_, _) => 0;

        /// <summary>
        /// Takes an ActiveNode input and outputs an int hash <br/>
        /// Used to determine uniqueness when deciding if this node has been reached before.<br/>
        /// <example>
        /// E.g. [Default] If you only care about the node you're on, use <c>(n) => n.Node.GetHashCode()</c> <br/>
        /// If you care about the direction you arrived on the node from, store that in memory. <br/>
        /// Then, incorporate the memory into the hash: <c>(n) => HashCode.Combine(n.Node, n.Memory)</c>
        /// </example>
        /// </summary>
        public Func<ActiveNode, int> UniqueVisitHash = (n) => n.Node.GetHashCode();

        public delegate bool ConnectivityDelegate(ActiveNode from, ActiveNode to);
        /// <summary>
        /// Takes two ActiveNode inputs (FROM and TO) and outputs a bool <br/>
        /// Indicates whether the target (TO) is reachable from your current node (FROM).<br/>
        /// <example>
        /// E.g. If you had a maze with '#' for walls, you could use <c>(_, to) => to.Value != '#'</c>
        /// </example>
        /// </summary>
        public ConnectivityDelegate ConnectivityTest = (_, _) => true;

        public delegate int CostPenaltyDelegate(ActiveNode from, ActiveNode to);
        /// <summary>
        /// Takes 2 ActiveNode inputs (FROM and TO) and outputs an int <br/>
        /// Indicates the additional cost of stepping onto the new TO tile. <br/>
        /// This is always added to the previous cost; do NOT include <c>+ from.Cost</c> yourself!. <br/>
        /// <example>
        /// E.g. [Default] If you want Cost to count steps, the new cost should be <c>(_, _) => 1</c> <br/>
        /// If you have an integer map for step penalty, then use <c>(_, to) => pathfinder.Map(to.Node)</c>
        /// </example>
        /// </summary>
        public CostPenaltyDelegate CostPenalty = (_,_) => 1;

        public delegate string MemoryDelegate(ActiveNode from, ActiveNode to);
        /// <summary>
        /// Takes two ActiveNode inputs (FROM and TO) and outputs a string, which becomes the memory of the TO node<br/>
        /// For storing arbitrary information you may want to use in ConnectivityRule, CostPenalty, or other parameters.<br/>
        /// The pathfinder by default does not use this information for anything.<br/>
        /// NOTE: Cannot incorporate Cost/Distance parameters into memory! <br/>
        /// <example>
        /// E.g. If your connectivity rule relies on the direction you entered from, you could store N/E/S/W here.<br/>
        /// </example>
        /// </summary>
        public MemoryDelegate NodeMemoryMap = (_, _) => string.Empty;

        public void SetTarget(TNode targetNode)
        {
            Target = targetNode;
            VictoryTest = on => on.Node.Equals(Target);
        }
        public void SetTarget(Func<ActiveNode, bool> predicate)
        {
            VictoryTest = predicate;
        }
        public void SetStart(TNode start)
        {
            Starts = new List<TNode>() { start };
        }
        public void SetStarts(IEnumerable<TNode> starts)
        {
            Starts = starts;
        }
        public bool LongestPath([MaybeNullWhen(false)] out ActiveNode path)
        {
            path = AllPaths().LastOrDefault();
            return path is not null;
        }
        public IEnumerable<ActiveNode> AllPaths()
        {
            ActiveNode found;
            if (VictoryTest is null) throw new ArgumentNullException("You need to set a target first. Use SetTarget() to do this.");
            if (Starts is null || !Starts.Any()) throw new ArgumentNullException("You need to set a start first. Use SetStart() to do this.");

            Dictionary<ActiveNode, HashSet<int>> visited = new();
            PriorityQueue<ActiveNode, int> queue = new();

            foreach (TNode start in Starts)
            {
                ActiveNode startNode = new(start, Map(start));
                if (Target is not null) startNode.Distance = DistanceMap(start, Target);
                queue.Enqueue(startNode, startNode.CostDistance);
                visited.Add(startNode, new());
            }
            int visitHash;
            while (queue.Count > 0)
            {
                found = queue.Dequeue();
                if (VictoryTest(found)) yield return found;
                foreach (ActiveNode adjacentNode in GetWalkable(found))
                {
                    visitHash = UniqueVisitHash(found);
                    if (visited[found].Contains(visitHash)) continue;
                    queue.Enqueue(adjacentNode, adjacentNode.CostDistance);
                    visited.Add(adjacentNode, new HashSet<int>() { visitHash });
                    visited[adjacentNode].UnionWith(visited[found]);
                }
                visited.Remove(found);
            }
        }
        public bool ShortestPath(out ActiveNode found)
        {
            found = null;
            if (VictoryTest is null) throw new ArgumentNullException("You need to set a target first. Use SetTarget() to do this.");
            if (Starts is null || !Starts.Any()) throw new ArgumentNullException("You need to set a start first. Use SetStart() to do this.");

            Dictionary<int, int> bestFound = new();
            PriorityQueue<ActiveNode, int> queue = new();
            HashSet<int> visited = new();

            foreach (TNode start in Starts)
            {
                ActiveNode startNode = new(start, Map(start));
                if (Target is not null) startNode.Distance = DistanceMap(start, Target);
                queue.Enqueue(startNode, startNode.CostDistance);
            }
            int visitHash;
            while (queue.Count > 0)
            {
                found = queue.Dequeue();
                visitHash = UniqueVisitHash(found);
                if (bestFound.TryGetValue(visitHash, out int best) && found.CostDistance > best) continue;
                if (VictoryTest(found)) return true;
                visited.Add(visitHash);
                foreach (ActiveNode adjacentNode in GetWalkable(found))
                {
                    visitHash = UniqueVisitHash(adjacentNode);
                    if (visited.Contains(visitHash)) continue;
                    if (!bestFound.TryGetValue(visitHash, out best) || adjacentNode.CostDistance < best)
                    {
                        bestFound[visitHash] = adjacentNode.CostDistance;
                        queue.Enqueue(adjacentNode, adjacentNode.CostDistance);
                    }
                }
            }
            return false;
        }
        public IEnumerable<ActiveNode> Reachable() => Reachable((_) => true);
        public IEnumerable<ActiveNode> Reachable(Func<ActiveNode, bool> exploreCondition)
        {
            if (VictoryTest is null) throw new ArgumentNullException("You need to set a target first. Use SetTarget() to do this.");
            if (Starts is null || !Starts.Any()) throw new ArgumentNullException("You need to set a start first. Use SetStart() to do this.");

            Dictionary<int, int> bestFound = new();
            PriorityQueue<ActiveNode, int> queue = new();
            HashSet<int> visited = new();

            foreach (TNode start in Starts)
            {
                ActiveNode startNode = new(start, Map(start));
                if (Target is not null) startNode.Distance = DistanceMap(start, Target);
                queue.Enqueue(startNode, startNode.CostDistance);
            }
            int visitHash;
            ActiveNode found;
            while (queue.Count > 0)
            {
                found = queue.Dequeue();
                visitHash = UniqueVisitHash(found);
                if (bestFound.TryGetValue(visitHash, out int best) && best < found.CostDistance) continue;
                if (exploreCondition(found)) yield return found;
                else continue;
                visited.Add(visitHash);
                foreach (ActiveNode adjacentNode in GetWalkable(found))
                {
                    visitHash = UniqueVisitHash(adjacentNode);
                    if (visited.Contains(visitHash)) continue;
                    if (!bestFound.TryGetValue(visitHash, out best) || adjacentNode.CostDistance < best)
                    {
                        bestFound[visitHash] = adjacentNode.CostDistance;
                        queue.Enqueue(adjacentNode, adjacentNode.CostDistance);
                    }
                }
            }
        }
        private IEnumerable<ActiveNode> GetWalkable(ActiveNode from)
        {
            foreach (TNode adjacent in from.Node.Adjacent())
            {
                ActiveNode to = new ActiveNode(adjacent, Map(adjacent));
                to.AssignPrevious(from, CostPenalty, NodeMemoryMap, DistanceMap, Target);
                if (ConnectivityTest(from, to)) yield return to;
            }
        }
    }
}
