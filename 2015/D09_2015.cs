using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D09_2015 : Day
    {
        class City
        {
            public string name;
            public Dictionary<City, int> distances;
            public City(string n)
            {
                name = n;
                distances = new Dictionary<City, int>();
            }
        }
        class Route
        {
            public City a, b;
            public int distance;
        }
        HashSet<City> locations = new();
        void MapCities()
        {
            locations.Clear();
            string[] input = GetInputForDay();
            for (int i = 0; i < input.Length; i++)
            {
                string[] paths = input[i].Split(' ');
                City a = locations.FirstOrDefault(c => c.name == paths[0]) ?? new City(paths[0]);
                City b = locations.FirstOrDefault(c => c.name == paths[2]) ?? new City(paths[2]);
                locations.Add(a);
                locations.Add(b);
                a.distances.Add(b, int.Parse(paths[4]));
                b.distances.Add(a, int.Parse(paths[4]));
            }
        }
        public override void PartA()
        {
            MapCities();
            City[] route = FindShortestRoute(locations.ToArray());
            PrintRoute(route);
            Submit(RouteLength(route));
        }
        public override void PartB()
        {
            MapCities();
            City[] route = FindLongestRoute(locations.ToArray());
            PrintRoute(route, false);
            Submit(RouteLength(route, false));
        }
        void PrintRoute(City[] route, bool goForShortest = true)
        {
            Console.WriteLine($"{route[0].name} to {route[^1].name} ({route[0].distances[route[^1]]})");
            for (int i = 0; i < route.Length - 1; i++)
            {
                Console.WriteLine($"{route[i].name} to {route[i+1].name} ({route[i].distances[route[i+1]]})");
            }
            Console.WriteLine($"Total distance: {RouteLength(route, goForShortest)}");
        }
        City[] FindShortestRoute(City[] route)
        {
            int shortestRoute = RouteLength(route);
            for (int x = 0; x < route.Length; x++)
            {
                for (int y = 0; y < route.Length; y++)
                {
                    if (x == y) continue; //can't swap a place with itself
                    route.Swap(x, y);
                    if (RouteLength(route) > shortestRoute)
                    {
                        route.Swap(x, y); //swap back if worse
                        continue;
                    }
                    if (RouteLength(route) < shortestRoute)
                    {
                        shortestRoute = RouteLength(route); //record new shortest
                    }
                }
            }
            return route;
        }
        City[] FindLongestRoute(City[] route)
        {
            int shortestRoute = RouteLength(route, false);
            for (int i = 0; i < route.Length; i++)
            {
                for (int x = 0; x < route.Length; x++)
                {
                    for (int y = 0; y < route.Length; y++)
                    {
                        if (x == y) continue; //can't swap a place with itself
                        route.Swap(x, y);
                        if (RouteLength(route, false) < shortestRoute)
                        {
                            route.Swap(x, y); //swap back if worse
                            continue;
                        }
                        if (RouteLength(route, false) > shortestRoute)
                        {
                            shortestRoute = RouteLength(route, false); //record new shortest
                            Console.WriteLine(shortestRoute);
                            i = 0;
                        }
                    }
                }
            }
            return route;
        }
        int RouteLength(City[] route, bool goForShortest = true)
        {
            int totalDistance = route[0].distances[route[^1]]; //connect end to start
            int longestHop = totalDistance;
            for (int i = 0; i < route.Length - 1; i++)
            {
                int hop = route[i].distances[route[i + 1]]; //connect city i to city i+1
                totalDistance += hop;
                if ((goForShortest && hop > longestHop) || (!goForShortest && hop < longestHop)) longestHop = hop;
            }
            return totalDistance - longestHop; //cut out longest hop because we want a path, not a loop
        }
    }
}
