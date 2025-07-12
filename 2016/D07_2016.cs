using System.Text.RegularExpressions;

namespace AOC.Y2016;

internal class D07_2016 : Day
{
    public override void PartOne()
    {
        Submit(Input.Count(SupportsTLS));
    }

    public override void PartTwo()
    {
        Submit(Input.Count(SupportsSSL));
    }
    private bool SupportsTLS(string ip)
    {
        bool insideBrackets = false;
        bool valid = false;
        foreach (string substring in ip.Split('[', ']'))
        {
            if (Regex.IsMatch(substring, "(?=(.)(?!\\1)(.)\\2\\1)"))
            {
                valid = true;
                if (insideBrackets) return false;
            }
            insideBrackets = !insideBrackets;
        }
        return valid;
    }
    private bool SupportsSSL(string ip)
    {
        HashSet<string> ABAs = [];
        HashSet<string> BABs = [];

        bool insideBrackets = true;
        foreach (string substring in ip.Split('[', ']'))
        {
            insideBrackets = !insideBrackets;
            string[] matches = Regex.Matches(substring, "(?=(.)(?!\\1)(.)\\1)").Select(x => substring.Substring(x.Index, 3)).ToArray();
            if (matches.Length == 0) continue;

            if (insideBrackets) BABs.UnionWith(matches);
            else ABAs.UnionWith(matches);

        }
        foreach (string aba in ABAs) if (BABs.Contains($"{aba[1]}{aba[0]}{aba[1]}")) return true;
        return false;
    }
}
