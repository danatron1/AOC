namespace AOC.Y2016;

internal class D09_2016 : Day
{
    public override void PartOne()
    {
        string compressed = InputLine;
        string decompressed = "";
        for (int i = 0; i < compressed.Length; i++)
        {
            if (char.IsWhiteSpace(compressed[i])) continue;
            if (IsMarker(compressed[i..], out string sectionToRepeat, out int repeats, out int endRepeatIndex))
            {
                decompressed += string.Concat(Enumerable.Repeat(sectionToRepeat, repeats));
                i += endRepeatIndex;
            }
            else decompressed += compressed[i];
        }
        Submit(decompressed.Length);
    }
    public override void PartTwo()
    {
        Submit(GetLength(InputLine));
    }
    private static long GetLength(string compressed)
    {
        long length = 0;
        for (int i = 0; i < compressed.Length; i++)
        {
            if (char.IsWhiteSpace(compressed[i])) continue;
            if (IsMarker(compressed[i..], out string sectionToRepeat, out int repeats, out int endRepeatIndex))
            {
                length += GetLength(sectionToRepeat) * repeats;
                i += endRepeatIndex;
            }
            else length++;
        }
        return length;
    }

    private static bool IsMarker(string section, out string sectionToRepeat, out int repeats, out int endRepeatIndex)
    {
        sectionToRepeat = "";
        repeats = 0;
        endRepeatIndex = 0;
        if (section[0] != '(') return false;

        int closeIndex = 1 + section.IndexOf(')');
        if (closeIndex == 0) return false;

        int[] markerInts = section[0..closeIndex].ExtractNumbers<int>();
        sectionToRepeat = section.Substring(closeIndex, markerInts[0]);
        repeats = markerInts[1];
        endRepeatIndex = closeIndex + sectionToRepeat.Length - 1;
        return true;
    }
}
