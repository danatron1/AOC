using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class StringExt
{
    public static int[] Digits(this string s, char zero = '0') => s.Select(c => c - zero).ToArray();

    /// <summary>
    /// Counts the number of times a given character appears in the source
    /// </summary>
    /// <param name="whole">the source to checked</param>
    /// <param name="section">the character(s) to be counted</param>
    /// <returns>the number of times section(s) appears in whole</returns>
    public static int FrequencyOf(this string whole, params char[] section)
    {
        int count = 0;
        foreach (char item in section)
        {
            count += whole.Length - whole.Replace(item.ToString(), "").Length;
        }
        return count;
    }
    /// <summary>
    /// Counts the number of times a given string appears in the source
    /// </summary>
    /// <param name="whole">the source to checked</param>
    /// <param name="section">the string(s) to be counted</param>
    /// <returns>the number of times section(s) appears in whole</returns>
    public static int FrequencyOf(this string whole, params string[] section)
    {
        int count = 0;
        foreach (string item in section)
        {
            count += (whole.Length - whole.Replace(item, "").Length) / item.Length;
        }
        return count;
    }
    public static int FrequencyOfOverlapping(this string whole, params string[] section)
    {
        //aaaabbba
        return section
            .Select(s => Enumerable.Range(0, 1 + whole.Length - s.Length)
                .Where(i => whole[i..(s.Length+i)] == s).Count()).Sum();
    }
    /// <summary>
    /// Same as .Split, except you can also convert the results to another type.
    /// </summary>
    /// <typeparam name="T">The type of the output array you want</typeparam>
    /// <param name="line">The string to be split</param>
    /// <param name="separators">The character(s) to split on</param>
    /// <returns>An array of the line, split up, in the requested datatype</returns>
    public static T[] SplitAs<T>(this string line, params char[]? separators)
    {
        if (separators is null || separators.Length == 0) separators = new char[] { ',' };
        string[] normalSplit = line.Split(separators, StringSplitOptions.TrimEntries);
        return normalSplit.ConvertTo<T>();
    }
    /// <summary>
    /// Same as .Split, except you can also convert the results to another type.
    /// </summary>
    /// <typeparam name="T">The type of the output array you want</typeparam>
    /// <param name="line">The string to be split</param>
    /// <param name="separators">The strings(s) to split on</param>
    /// <returns>An array of the line, split up, in the requested datatype</returns>
    public static T[] SplitAs<T>(this string line, params string[]? separators)
    {
        if (separators is null || separators.Length == 0) separators = new string[] { "," };
        string[] normalSplit = line.Split(separators, StringSplitOptions.TrimEntries);
        return normalSplit.ConvertTo<T>();
    }
    public static string Repeat(this string line, int times)
    {
        if (line.Length == 0 || times == 0) return string.Empty;
        if (line.Length == 1) return new string(line[0], times);
        return string.Concat(Enumerable.Repeat(line, times));
    }
    public static string Overlap(this string a, string b, bool orderMatters = true)
    {
        if (!orderMatters) return string.Concat(a.Intersect(b));
        if (a.Length > b.Length) Utility.RefSwap(ref a, ref b);
        return string.Concat(a.Where((e, i) => Equals(e, b.ElementAt(i))));
    }
    public static bool TryExtractNumberLocation(this string s, out int start, out int end)
    {
        start = s.FirstIndex(char.IsDigit, out _);
        end = start;
        if (start == -1) return false;
        //find end of numbers
        while (++end < s.Length && char.IsDigit(s[end])) ;
        //if there's a decimal point, include it and continue until no more numbers
        if (end < s.Length && s[end] == '.') while (++end < s.Length && char.IsDigit(s[end])) ;
        //include - sign if there is one.
        if (start > 0 && s[start - 1] == '-') start--;
        return true;
    }
    public static bool TryExtractNumber(this string s, out double number)
    {
        number = 0;
        if (!s.TryExtractNumberLocation(out int start, out int end)) return false;
        number = double.Parse(s[start..end]);
        return true;
    }
    public static double ExtractNumber(this string s)
    {
        s.TryExtractNumberLocation(out int start, out int end);
        if (s[end] == '.') while (++end < s.Length && char.IsDigit(s[end])) ;
        return (start > 0 && s[start - 1] == '-') ? -double.Parse(s[start..end]) : double.Parse(s[start..end]);
    }
    public static double[] ExtractNumbers(this string s)
    {
        List<double> numbers = new();
        while (s.TryExtractNumberLocation(out int start, out int end))
        {
            numbers.Add(double.Parse(s[start..end]));
            s = s[(end + 1)..];
        }
        return numbers.ToArray();
    }

    #region Pull Columns
    public static IEnumerable<string> PullColumns(this string sentence, params int[] relevantColumnIndexes)
    {
        return sentence.PullColumns(" ", relevantColumnIndexes);
    }
    public static IEnumerable<string> PullColumns(this string sentence, string splitOn, params int[] relevantColumnIndexes)
    {
        return sentence.Split(splitOn).Where((s, i) => relevantColumnIndexes.Contains(i));
    }
    public static IEnumerable<IEnumerable<string>> PullColumns(this IEnumerable<string> sentences, params int[] relevantColumnIndexes)
    {
        return sentences.PullColumns(" ", relevantColumnIndexes);
    }
    public static IEnumerable<IEnumerable<string>> PullColumns(this IEnumerable<string> sentences, string splitOn, params int[] relevantColumnIndexes)
    {
        foreach (string sentence in sentences)
        {
            yield return sentence.Split(splitOn).Where((s, i) => relevantColumnIndexes.Contains(i));
        }
    }
    public static string PullColumn(this string sentence, int columnIndex) => sentence.PullColumn(" ", columnIndex);
    public static string PullColumn(this string sentence, string splitOn, int columnIndex)
    {
        return sentence.Split(splitOn)[columnIndex];
    }
    public static IEnumerable<string> PullColumn(this IEnumerable<string> sentences, int columnIndex)
    {
        return sentences.PullColumn(" ", columnIndex);
    }
    public static IEnumerable<string> PullColumn(this IEnumerable<string> sentences, string splitOn, int columnIndex)
    {
        foreach (string sentence in sentences) yield return sentence.PullColumn(splitOn, columnIndex);
    }
    #endregion
    public static string ReplaceInstance(this string source, string toReplace, string replaceWith, int instance)
    {
        if (toReplace == replaceWith) return source;
        if (instance < 0) throw new ArgumentOutOfRangeException($"{nameof(instance)} cannot have a value below 0 {instance}");
        int index = source.IndexOfInstance(toReplace, instance);
        if (index < 0) return source;
        return source[..index] + replaceWith + source[(index+ toReplace.Length)..];
        
    }
    public static int IndexOfInstance(this string source, string toSearch, int instance)
    {
        //abbbcbb
        for (int index = -1; instance >= 0; instance--)
        {
            int newIndex = source[++index..].IndexOf(toSearch);
            if (newIndex < 0) break;
            index += newIndex;
            if (instance == 0) return index;
        }
        return -1;
    }
}
