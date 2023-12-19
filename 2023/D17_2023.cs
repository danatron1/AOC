using AOC.Items;
using AOC.Items.Pathfinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D17_2023 : Day<int>
{

    int minToTurn = 0, maxBeforeTurn = 0;
    public override void PartA()
    {
        //useExampleInput = true;
        maxBeforeTurn = 3;

        Grid<int> grid = new Grid<int>(Input2D);
        Pathfinder<Point2D, int> pathfinder = grid.Pathfinder();
        pathfinder.SetStart(grid.GetCorners().TopLeft);
        pathfinder.SetTarget(grid.GetCorners().BottomRight);
        pathfinder.CostPenalty = (p, n) => p.Cost + pathfinder.Map(n.Node);
        pathfinder.UniqueVisitHash = (n) => HashCode.Combine(n.Node, n.Memory);
        pathfinder.NodeMemoryMap = GetMemory;
        pathfinder.ConnectivityTest = ConnectivityTest;

        if (pathfinder.FindPath(out var found))
        {
            //TraceAndPrintPath(grid, found);
            Submit(found.Cost);
        }
    }
    internal string GetMemory(Pathfinder<Point2D, int>.ActiveNode from, Pathfinder<Point2D, int>.ActiveNode to)
    {
        string memory = from.Memory + to.Node.DirectionFrom(from.Node)!.Value.ToChar();
        if (memory.Length > maxBeforeTurn) memory = memory[1..];
        return memory;
    }
    private void TraceAndPrintPath(Grid<int> grid, Pathfinder<Point2D, int>.ActiveNode? found)
    {
        if (found is null) return;
        Console.WriteLine($"Found solution with heat loss of {found.Cost}");
        grid.printedRepresentation['N'] = "^";
        grid.printedRepresentation['E'] = ">";
        grid.printedRepresentation['S'] = "v";
        grid.printedRepresentation['W'] = "<";
        while (found is not null)
        {
            if (found.Memory != string.Empty) grid[found.Node] = found.Memory[^1];
            found = found.Previous;
        }
        grid.PrintBoard();
    }
    bool ConnectivityTest(Pathfinder<Point2D, int>.ActiveNode from, Pathfinder<Point2D, int>.ActiveNode to)
    {
        if (to.Value == 0) return false; //out of range.
        if (from.Previous is null) return true; //start node = free reign
        if (to.Node == from.Previous.Node) return false; //can't go backwards
        if (from.Memory == to.Memory) return false; //can't go more than 3 in straight line
        bool tryingToTurn = from.Memory[^1] != to.Memory[^1];
        int straightLine = ConsecutiveSteps(from.Memory);
        if (straightLine < minToTurn && tryingToTurn) return false; //for part 2
        return true;
    }
    private int ConsecutiveSteps(string memory)
    {
        int i = 1;
        while (i++ < memory.Length && memory[^i] == memory[^1]) ;
        return i-1;
    }

    public override void PartB()
    {
        useExampleInput = true;
        maxBeforeTurn = 10;
        minToTurn = 4;

        Grid<int> grid = new Grid<int>(Input2D);
        Pathfinder<Point2D, int> pathfinder = grid.Pathfinder();
        pathfinder.SetStart(grid.GetCorners().TopLeft);
        pathfinder.SetTarget(grid.GetCorners().BottomRight);
        pathfinder.CostPenalty = (p, n) => p.Cost + pathfinder.Map(n.Node);
        pathfinder.UniqueVisitHash = (n) => HashCode.Combine(n.Node, n.Memory);
        pathfinder.NodeMemoryMap = GetMemory;
        pathfinder.ConnectivityTest = ConnectivityTest;
        pathfinder.SetTarget(x => x.Node.Equals(pathfinder.Target) && x.Memory[^minToTurn..].Distinct().Count() == 1);

        if (pathfinder.FindPath(out var found))
        {
            TraceAndPrintPath(grid, found);
            Submit(found.Cost);
        }
    }
}
