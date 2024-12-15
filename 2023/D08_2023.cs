using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D08_2023 : Day
{
    class Node
    {
        public static Dictionary<string, Node> nodeFinder = new();
        public string name;
        string _left, _right;
        Node? _leftNode, _rightNode;
        public Node left
        {
            get
            {
                _leftNode ??= nodeFinder[_left];
                return _leftNode;
            }
        }
        public Node right
        {
            get
            {
                _rightNode ??= nodeFinder[_right];
                return _rightNode;
            }
        }
        public Node(string line)
        {
            name = line[..3];
            _left = line[7..10];
            _right = line[12..15];
            nodeFinder[name] = this;
        }
    }
    public override void PartOne()
    {
        for (int i = 2; i < Input.Length; i++)
        {
            new Node(Input[i]);
        }
        int steps = 0;
        Node current = Node.nodeFinder["AAA"];
        while (current.name != "ZZZ")
        {
            current = Input[0][steps % Input[0].Length] == 'L' ? current.left : current.right;
            steps++;
        }
        Submit(steps);
    }
    public override void PartTwoSetup()
    {
        Node.nodeFinder = new Dictionary<string, Node>();
    }
    public override void PartTwo()
    {
        for (int i = 2; i < Input.Length; i++)
        {
            new Node(Input[i]);
        }
        Node[] starts = Node.nodeFinder.Values.Where(x => x.name.EndsWith('A')).ToArray();
        //int[] offsets = new int[starts.Length];
        long[] cycles = new long[starts.Length];
        for (int i = 0; i < starts.Length; i++)
        {
            int steps = 0;
            Node current = starts[i];
            while (!current.name.EndsWith('Z'))
            {
                current = Input[0][steps % Input[0].Length] == 'L' ? current.left : current.right;
                steps++;
            }
            cycles[i] = steps;
        }
        Submit(cycles.LCM());
    }
}
