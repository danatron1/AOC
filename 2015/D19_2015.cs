using AOC.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static AOC.Y2015.D19_2015;

namespace AOC.Y2015;

internal class D19_2015 : Day
{
    public override void PartASetup()
    {
        known = new HashSet<Molecule>();
        knownAtoms = new HashSet<Atom>();
        transformations = new List<Transform>();
        Molecule.medicine = new(Input[^1]);
        Molecule.start = new("e");
        for (int i = 0; i < Input.Length - 2; i++)
        {
            string[] columns = Input[i].PullColumns(0, 2).ToArray();
            knownAtoms.UnionWith(columns.SelectMany(a => Atom.GetAtoms(a)));
            transformations.Add(new(columns[0], columns[1]));
        }
        transformations = transformations.OrderByDescending(x => (x.AtomsTo.Count + x.AtomsFrom.Count)).ToList();
    }
    HashSet<Atom> knownAtoms;
    HashSet<Molecule> known;
    List<Transform> transformations;
    public override void PartA()
    {
        Submit(PossibleNextSteps(Molecule.medicine).Distinct().Count());
    }
    string bestSoFar = "";
    int skippedNodes = 0;
    Molecule? winner = null;
    IEnumerable<Node> PossibleNextSteps(Node from)
    {
        Molecule fromMolecule = from.Formula();
        if (fromMolecule == Molecule.medicine) winner = fromMolecule;
        if (fromMolecule.StepsToReach >= Maximum) yield break;
        if (!topDown && fromMolecule.Formula.Length >= Molecule.medicine.Formula.Length) yield break;
        if ((topDown && fromMolecule.Formula.Length <= bestSoFar.Length) ||
            (!topDown && fromMolecule.Formula.Length >= bestSoFar.Length))
        {
            bestSoFar = fromMolecule.Formula;
            Console.WriteLine($"{Node.ToExploreCount()}/{known.Count} (skipped {skippedNodes}) - best {bestSoFar.Length} ({(bestSoFar.Length < 40 ? bestSoFar : "long")}) - {from}");
        }
        if (known.TryGetValue(fromMolecule, out Molecule previouslyExplored))
        {
            if (previouslyExplored.StepsToReach <= fromMolecule.StepsToReach) yield break;
            throw new Exception("How");
        }
        /*Must keep! It's how I do win detection*/
        known.Add(fromMolecule);
        foreach (Transform t in transformations)
        {
            for (int i = 0; i < fromMolecule.Formula.FrequencyOfOverlapping(t.From); i++)
            {
                yield return new Node(t, i, from);
            }
        }
    }
    IEnumerable<Molecule> PossibleNextSteps(Molecule from)
    {
        foreach (var t in transformations)
        {
            for (int i = 0; i < from.Formula.FrequencyOfOverlapping(t.From); i++)
            {
                Molecule newMol = new(from.Formula.ReplaceInstance(t.From, t.To, i), from.StepsToReach+1);
                yield return newMol;
            }
        }
    }
    IEnumerable<string> PossibleNextSteps(string from)
    {
        foreach (var t in transformations)
        {
            for (int i = 0; i < from.FrequencyOfOverlapping(t.From); i++)
            {
                yield return from.ReplaceInstance(t.From, t.To, i);
            }
        }
    }
    bool topDown = false;
    void InvertTransformations()
    {
        topDown = !topDown;
        Utility.RefSwap(ref Molecule.medicine, ref Molecule.start);
        transformations = transformations.Select(x => new Transform(x.To, x.From)).ToList();
        bestSoFar = Molecule.start.Formula;
    }
    public override void PartBSetup()
    {
        known.Clear();
        transformations.RemoveAll(x => x.AtomsFrom.Union(x.AtomsTo).Any(c => c.Name == "C"));
    }
    public override void PartB()
    {
        #region Attempt 1
        /*
        Queue<Molecule> toCheck = new();
        Molecule current = new ("e");
        toCheck.Enqueue(current);
        long stepsTaken = 0;
        while (current != medicine)
        {
            current = toCheck.Dequeue();
            stepsTaken++;
            if (stepsTaken % 1000 == 1) Console.WriteLine($"{toCheck.Count}/{known.Count} - {current.Formula.Length}/{medicine.Formula.Length} - {stepsTaken++}");
            foreach (Molecule item in PossibleNextSteps(current))
            {
                if (item == medicine)
                {
                    current = item;
                    break;
                }
                if (item.Formula.Length >= medicine.Formula.Length) continue;
                toCheck.Enqueue(item);
                known.Add(item);
            }
            if (current == medicine) break;
        }
        Submit(current.StepsToReach);
        */
        #endregion
        #region Attempt 2
        /*
        Queue<Molecule> toCheck = new();
        Molecule current = medicine;
        medicine = new("e");
        toCheck.Enqueue(current);
        long stepsTaken = 0;
        int stepProgress = -1;
        while (current != medicine)
        {
            current = toCheck.Dequeue();
            stepsTaken++;
            if (current.StepsToReach > stepProgress)
            {
                known.RemoveWhere(x => x.StepsToReach < stepProgress);
                stepProgress++;
                Console.WriteLine($"In memory {toCheck.Count}/{known.Count} - Length {current.Formula.Length}/{medicine.Formula.Length} - Steps {stepProgress} - Iterations {stepsTaken}");
            }
            //Console.ReadLine();
            foreach (Molecule item in PossibleNextSteps(current))
            {
                if (item == medicine)
                {
                    current = item;
                    break;
                }
                if (item.Formula.Contains('e')) continue;
                if (known.Contains(item)) continue;
                toCheck.Enqueue(item);
                known.Add(item);
            }
            if (current == medicine) break;
        }
        Submit(current.StepsToReach);
        */
        #endregion
        #region Attempt 3
        /*
         * New strategy:
         * Instead of strings, store lists to reference-type nodes.
         * Each node is a transformation, and you can get the string by following the known path
         * for each possible next step, 
         *   (maybe) skip if we've already got to the resulting string.
         *  create a new list with the new node appended.
         *  the new list should just be a reference to the old list with an additional transformation
         *
         * Phase 2:
         * Improve efficiency by counting distinct atoms, calculate minimum changes needed;
         * e.g. AtomsOfTarget - AtomsOfCurrent = MovesLeft
         *      If (MovesLeft > AtomDifference) impossible
         *
        //InvertTransformations();
        Node.useStack = false;
        Node.AddToExplore(new Node());
        while (!winner.HasValue)
        {
            Node n = Node.GetToExplore();
            foreach (Node item in PossibleNextSteps(n))
            {
                if (item.MaxRemainingMoves < item.TheoreticalMinMovesToMatch(Molecule.medicine))
                {
                    skippedNodes++;
                    continue;
                }
                Node.AddToExplore(item);
            }
        }
        if (known.TryGetValue(Molecule.medicine, out Molecule solution))
        {
            Submit(solution.StepsToReach);
        }*/
        #endregion
        Submit(FindPath(Molecule.start.Formula, Molecule.medicine.Formula));
    }
    int highestSeen = 0;
    public int FindPath(string from, string to)
    {
        Dictionary<string, int> distances = new();
        HashSet<string> known = new();
        PriorityQueue<string, int> queue = new();
        distances[from] = 0;
        queue.Enqueue(from, from.Length);
        while (queue.Count > 0)
        {
            string cell = queue.Dequeue();
            if (known.Contains(cell)) continue;
            int cellDist = distances[cell];
            if (Equals(cell, to)) return cellDist;
            known.Add(cell);
            foreach (string next in PossibleNextSteps(cell))
            {
                int weight = next.Length;
                var newScore = cellDist + weight;
                if (newScore > highestSeen)
                {
                    highestSeen = newScore;
                    Console.WriteLine($"{highestSeen} - {queue.Count}/{known.Count} - {cell.Length}");
                }
                if (!known.Contains(next)) queue.Enqueue(next, next.Length);
                if (!distances.TryGetValue(next, out int score) || newScore < score) distances[next] = newScore;
            }
        }
        return -1;
    }
    internal class Node
    {
        internal Transform Transform;
        public Tally<Atom> Atoms;
        public int Instance;
        public int StepsToReach = 0;
        public int MaxRemainingMoves;
        public Node? Prev;
        public Node() { }
        public Node(Transform transform, int instance, Node prev)
        {
            Transform = transform;
            Instance = instance;
            Prev = prev;
            Atoms = AfterTransform(prev);
            Atoms.CanContainNegative = false;
            StepsToReach = 1 + prev.StepsToReach;
            MaxRemainingMoves = Molecule.medicine.Atoms.Count() - (int)Atoms.CountAll();
        }
        private Tally<Atom> AfterTransform(Node prev)
        {
            if (prev.Atoms is null) return new Tally<Atom>();
            return prev.Atoms + Transform.AtomsTo.ToTally() - Transform.AtomsFrom.ToTally();
        }

        public Molecule Formula()
        {
            if (Prev == null) return Molecule.start;
            return Prev.Formula().Mutate(this);
        }
        public override string ToString() => $"{StepsToReach}: {Transform}";
        public static bool useStack = false;
        static Queue<Node> toExploreQueue = new();
        static Stack<Node> toExploreStack = new();


        public static void AddToExplore(Node next)
        {
            if (useStack) toExploreStack.Push(next);
            else toExploreQueue.Enqueue(next);
        }
        public static Node GetToExplore()
        {
            if (useStack) return toExploreStack.Pop();
            else return toExploreQueue.Dequeue();
        }
        internal static int ToExploreCount()
        {
            if (useStack) return toExploreStack.Count;
            else return toExploreQueue.Count;
        }

        internal int TheoreticalMinMovesToMatch(Molecule other)
        {
            return (int)((Atoms - other.Atoms.ToTally()).CountAll());
        }
    }
    internal struct Transform
    {
        public List<Atom> AtomsFrom;
        public List<Atom> AtomsTo; 
        public string From, To;
        public Transform(string from, string to)
        {
            From = from;
            To = to;
            AtomsFrom = Atom.GetAtoms(from).ToList();
            AtomsTo = Atom.GetAtoms(to).ToList();
        }
        public override string ToString() => $"{From} => {To}";
    }
    internal struct Molecule : IEqualityComparer<Molecule>, IEquatable<Molecule>
    {
        public string Formula;
        public int StepsToReach;
        public static Molecule medicine;
        public static Molecule start;
        public List<Atom> Atoms => Atom.GetAtoms(Formula);
        public Molecule(string formula) : this(formula, 0) { }
        public Molecule(string formula, int stepsToReach)
        {
            Formula = formula;
            StepsToReach = stepsToReach;
        }
        public static bool operator ==(Molecule a, Molecule b) => a.Equals(b);
        public static bool operator !=(Molecule a, Molecule b) => !a.Equals(b);
        public bool Equals(Molecule other) => Formula.Equals(other.Formula);
        public bool Equals(Molecule x, Molecule y) => x.Equals(y);
        public int GetHashCode([DisallowNull] Molecule obj) => obj.GetHashCode();
        public override bool Equals(object? obj)
        {
            if (obj is string s) return s.Equals(Formula);
            return obj is Molecule molecule && Equals(molecule);
        }
        public override int GetHashCode() => Formula.GetHashCode();
        public override string ToString() => Formula;
        internal Molecule Mutate(Node n)
        {
            StepsToReach++;
            Formula = Formula.ReplaceInstance(n.Transform.From, n.Transform.To, n.Instance);
            return this;
        }
    }
    internal struct Atom
    {
        public string Name;
        public Atom(string name) => Name = name;
        public override string ToString() => Name;
        public static List<Atom> GetAtoms(string source)
        {
            List<Atom> atoms = new();
            string newAtom;
            while (source.Length > 0)
            {
                newAtom = source[0].ToString();
                if (source.Length > 1 && char.IsLower(source[1])) newAtom += source[1];
                source = source[newAtom.Length..];
                atoms.Add(new(newAtom));
            }
            return atoms;
        }
    }
}
