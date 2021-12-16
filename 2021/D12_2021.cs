using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D12_2021 : Day
    {
        class Cave
        {
            public bool large { get; init; }
            public bool start { get; init; }
            public bool end { get; init; }
            public string name { get; init; }
            public int code { get; init; }
            public List<Cave> connections = new List<Cave>();
            public Cave(string name, int offset)
            {
                code = 1 << offset;
                start = name == "start";
                end = name == "end";
                large = char.IsUpper(name[0]);
                if (start || end || large) code = 0;
                this.name = name;
            }
            public override string ToString()
            {
                return $"{(large ? "Large cave" : "Cave")} {name} has {connections.Count} connections.";
            }
        }
        List<Cave> caves;
        public override void Solve() => PartB();
        public override void PartA()
        {
            MapCaves();
            int routes = FindAllRoutes();
            Console.WriteLine($"Total routes found:");
            Copy(routes);
        }
        void MapCaves(bool example = false)
        {
            caves = new List<Cave>();
            string[] input = GetInputForDay(example: example);
            int i = 0;
            foreach (string item in input)
            {
                string[] halves = item.Split('-');
                foreach (string half in halves)
                {
                    if (!caves.Any(c => c.name == half))
                    {
                        Cave newCave = new Cave(half, i);
                        caves.Add(newCave);
                        if (newCave.code > 0) i++;
                        
                    }
                }
                Cave left = caves.First(c => c.name == halves[0]);
                Cave right = caves.First(c => c.name == halves[1]);
                left.connections.Add(right);
                right.connections.Add(left);
            }
        }
        class Route
        {
            //public HashSet<Cave> visited;
            public int visits;
            public Cave lastVisit;
            public bool doubleVisit;
            public bool Visited(Cave c) => (visits & c.code) != 0;
            public Route(Cave start, bool doubleVisit)
            {
                //visited = new HashSet<Cave>();
                visits = 0;
                //visited.Add(start);
                lastVisit = start;
                this.doubleVisit = doubleVisit;
            }
            public Route(int alreadyVisited, bool alreadyDoubleVisited, Cave cave)
            {
                visits = alreadyVisited;
                doubleVisit = alreadyDoubleVisited || (!cave.large && Visited(cave));
                visits |= cave.code;
                lastVisit = cave;
            }
            
            //public Route(Route route, Cave cave)
            //{
            //    visited = new HashSet<Cave>();
            //    visited.UnionWith(route.visited);
            //    doubleVisit = route.doubleVisit || (!cave.large && visited.Contains(cave));
            //    lastVisit = cave;
            //    if (!cave.large) visited.Add(cave);
            //}
            //public override string ToString()
            //{
            //    string s = doubleVisit.ToString()[0].ToString();
            //    foreach (Cave cave in visited) s += '-' + cave.name;
            //    return s;
            //}
        }
        private int FindAllRoutes(bool doubleVisitOneSmallCave = false)
        {
            int visited, finishedRoutes = 0;
            Queue<Route> routes = new Queue<Route>();
            routes.Enqueue(new Route(caves.First(n => n.start), !doubleVisitOneSmallCave));
            Route route;
            HashSet<Cave> next;
            while (routes.Count > 0)
            {
                route = routes.Dequeue();
                visited = route.visits;
                next = route.lastVisit.connections
                    .Where(c => !c.start && !(!c.large && route.doubleVisit && route.Visited(c))).ToHashSet();
                foreach (Cave cave in next)
                {
                    if (cave.end) finishedRoutes++;
                    else routes.Enqueue(new Route(visited, route.doubleVisit, cave));
                }
            }
            return finishedRoutes;
        }
        //private int FindAllRoutes(bool doubleVisitOneSmallCave = false)
        //{
        //    int finishedRoutes = 0;
        //    Queue<Route> routes = new Queue<Route>();
        //    routes.Enqueue(new Route(caves.First(n => n.start), !doubleVisitOneSmallCave));
        //    Route route;
        //    HashSet<Cave> next;
        //    while (routes.Count > 0)
        //    {
        //        route = routes.Dequeue();
        //        next = route.lastVisit.connections
        //            .Where(c => !c.start && !(!c.large && route.doubleVisit && route.visited.Contains(c))).ToHashSet();
        //        foreach (Cave cave in next)
        //        {
        //            if (cave.end) finishedRoutes++;
        //            else routes.Enqueue(new Route(route, cave));
        //        }
        //    }
        //    return finishedRoutes;
        //}
        public override void PartB()
        {
            MapCaves(true);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int routes = FindAllRoutes();
            sw.Stop();
            Console.WriteLine($"Optimised time: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"Total routes found:");
            Copy(routes);
            // 10728 too low
        }

    }
}
