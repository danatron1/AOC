namespace AOC.Y2015;
internal class D19_2015 : Day
{
    List<(string, string)> transformations = new();
    string start = "e";
    string target;
    public override void PartASetup()
    {
        target = Input[^1];
        for (int i = 0; i < Input.Length - 2; i++)
        {
            string[] relevantColumns = Input[i].PullColumns(0, 2).ToArray();
            transformations.Add((relevantColumns[0], relevantColumns[1]));
        }
    }
    void InvertDirection()
    {
        Utility.RefSwap(ref start, ref target);
        transformations = transformations.Select(x => (x.Item2, x.Item1)).ToList();
    }
    public override void PartA()
    {
        Submit(NextSteps(target).Distinct().Count());
    }
    IEnumerable<string> NextSteps(string current)
    {
        foreach (var (From, To) in ValidTransformations(current))
        {
            for (int i = 0; i < current.FrequencyOfOverlapping(From); i++)
            {
                yield return current.ReplaceInstance(From, To, i);
            }
        }
    }
    IEnumerable<(string, string)> ValidTransformations(string current)
    {
        foreach (var (From, To) in transformations)
        {
            if (current.Contains(From)) yield return (From, To);
        }
    }
    public override void PartB()
    {
        InvertDirection();
        int lowestSeen = int.MaxValue, stepsToReach = 0, iterations = 0;
        transformations.Shuffle();
        foreach ((string, string)[] item in transformations.Permutations())
        {
            (string, string)[] theseTransforms = item;
            string molecule = start;
            iterations += stepsToReach;
            stepsToReach = 0;
            while (true)
            {
                var t = theseTransforms.Where(x => molecule.Contains(x.Item1)).FirstOrDefault();
                if (t == default) break;
                molecule = molecule.ReplaceInstance(t.Item1, t.Item2, 0);
                stepsToReach++; iterations++;
            }
            if (molecule.Length < lowestSeen)
            {
                lowestSeen = molecule.Length;
                Console.WriteLine($"{iterations} Found length {lowestSeen} molecule {molecule} in {stepsToReach} steps");
                if (molecule == target) break;
            }
        }
        Submit(start);
    }
}
