namespace AOC.Y2016;

internal class D03_2016 : Day
{
    int ValidTriangles(IEnumerable<string> input)
    {
        return input.Select(x => x.ExtractNumbers()).Where(x => (x.Max() * 2) < x.Sum()).Count();
    }
    public override void PartOne()
    {
        Submit(ValidTriangles(Input));
    }
    public override void PartTwo()
    {
        List<string> triangles = new();
        for (int i = 0; i < Input.Length; i += 3)
        {
            int[][] row = { Input[i].ExtractNumbers<int>(), Input[i + 1].ExtractNumbers<int>(), Input[i + 2].ExtractNumbers<int>() };
            triangles.Add($"{row[0][0]} {row[1][0]} {row[2][0]}");
            triangles.Add($"{row[0][1]} {row[1][1]} {row[2][1]}");
            triangles.Add($"{row[0][2]} {row[1][2]} {row[2][2]}");
        }
        Submit(ValidTriangles(triangles));
    }
}
