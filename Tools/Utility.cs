using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class Utility
{
    public const string folderPath = @"C:\Users\Danatron1\source\repos\AOC";
    public static string NameFor(int day, int year) => $"D{day.ToString().PadLeft(2, '0')}_{year}";
    public static void Swap<T>(this T[] array, int location1, int location2)
    {
        (array[location2], array[location1]) = (array[location1], array[location2]);
    }
    public static Random rng = new Random();
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            array.Swap(i, rng.Next(i));
        }
    }
    public static string InputFilepath(int day, int year, bool example = false)
    {
        string pathFull = $@"{folderPath}\Inputs\{year}";
        Directory.CreateDirectory(pathFull);
        pathFull += $@"\{NameFor(day, year)}_{(example ? "Example" : "Input")}.txt";
        return pathFull;
    }
    public static int[] Digits(this string s, char zero = '0') => s.Select(c => c - zero).ToArray();
    
    public static void Copy(object item)
    {
        if (item == null) return;
        Cmd($"echo | set /p={item}| clip");
    }
    public static string Cmd(string command)
    {
        command = command.Replace("\"", "\\\"");
        return Run("cmd.exe", $"/c \"{command}\"");
    }
    public static string Run(string filename, string arguments = "")
    {
        Process process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
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
    /// <param name="whole">the source to read</param>
    /// <param name="section">the thing to be counted</param>
    /// <returns>the number of times section appears in whole</returns>
    public static int FrequencyOf(this string whole, params string[] section)
    {
        int count = 0;
        foreach (string item in section)
        {
            count += (whole.Length - whole.Replace(item, "").Length) / item.Length;
        }
        return count;
    }
    public static T[] Split<T>(this string line, params char[]? seperators)
    {
        if (seperators is null || seperators.Length == 0) seperators = new char[] { ',' };
        string[] normalSplit = line.Split(seperators, StringSplitOptions.TrimEntries);
        return normalSplit.ConvertTo<T>();

    }
    public static T[] ConvertTo<T>(this object[] array, bool forceMatchingLength = false)
    {
        List<T> converted = new List<T>();
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        for (int i = 0; i < array.Length; i++)
        {
            object? ob = converter.ConvertFrom(array[i]);
            if (ob is T t) converted.Add(t);
            else if (forceMatchingLength) converted.Add(default);
        }
        return converted.ToArray();
    }
    public static Array ConvertTo<T>(this Array array, bool forceMatchingLength = false)
    {
        int[] limits = new int[array.Rank];
        int[] lower = new int[array.Rank];
        for (int l = 0; l < limits.Length; l++)
        {
            limits[l] = array.GetLength(l);
            lower[l] = array.GetLowerBound(l);
        }
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        Array converted = Array.CreateInstance(typeof(T), limits, lower);
        int i = 0;
        int[] indices = new int[limits.Length];
        foreach (object item in array)
        {
            int size = i++;
            for (int pos = indices.Length - 1; pos >= 0; pos--)
            {
                indices[pos] = size % limits[pos];
                size /= limits[pos];
            }
            object? ob = converter.ConvertFrom(item);
            if (ob is T t) converted.SetValue(t, indices);
            else if (forceMatchingLength) converted.SetValue(default, indices);
        }
        return converted;
    }
    /// <summary>
    /// Finds all possible combinations of array elements.
    /// For example, [A,B,C] returns [[],[A],[B],[C],[A,B],[A,C],[B,C],[A,B,C]]
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="source">The array to find combinations in</param>
    /// <param name="allowedLengths">If a value is provided, will limit results to those with the given length</param>
    /// <returns>An IEnumerable list of arrays</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, Range? allowedLengths = null)
    {
        //explanation here https://stackoverflow.com/a/57058345
        if (source.Count() >= 30) throw new ArgumentOutOfRangeException("Source array too large");
        if (null == source) throw new ArgumentNullException(nameof(source));
        T[] data = source.ToArray();
        int minlength = 0, maxlength = 0;
        if (allowedLengths.HasValue)
        {
            minlength = allowedLengths.Value.Start.IsFromEnd ? source.Count() - allowedLengths.Value.Start.Value : allowedLengths.Value.Start.Value;
            maxlength = allowedLengths.Value.End.IsFromEnd ? source.Count() - allowedLengths.Value.End.Value : allowedLengths.Value.End.Value;
        }
        foreach (IEnumerable<T> item in Enumerable
          .Range(0, 1 << (data.Length))
          .Select(index => data
             .Where((v, i) => (index & (1 << i)) != 0)))
        {
            if (!allowedLengths.HasValue || (item.Count() <= maxlength && item.Count() >= minlength))
            {
                yield return item;
            }
        } 
    }
    /// <summary>
    /// Same as .Select, but you also have access to the previous element in the array.
    /// E.g. array.SelectWithPrevious((prev, curr) => prev + curr);
    /// Returned array will be 1 shorter than the input, as element 1 is skipped.
    /// If [1, 2, 3] was passed in with the above example, would return [3, 5]
    /// </summary>
    /// <typeparam name="TSource">Type of the source array</typeparam>
    /// <typeparam name="TResult">Type of the returned values</typeparam>
    /// <param name="source">The array to iterate over</param>
    /// <param name="projection">Function to use</param>
    /// <returns>An IEnumerable after the Function has been applied to each element. </returns>
    public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>
    (this IEnumerable<TSource> source,
     Func<TSource, TSource, TResult> projection)
    {
        using (var iterator = source.GetEnumerator())
        {
            if (!iterator.MoveNext())
            {
                yield break;
            }
            TSource previous = iterator.Current;
            while (iterator.MoveNext())
            {
                yield return projection(previous, iterator.Current);
                previous = iterator.Current;
            }
        }
    }
    /// <summary>
    /// Returns all pairs of indexes for the given length
    /// E.g. (if array has length 3) array.IndexPairs() => [0, 1], [0, 2], [1, 2]
    /// </summary>
    /// <typeparam name="T">Type of the array</typeparam>
    /// <param name="array">The array to find index pairs for</param>
    /// <param name="groupsOf">The length of each returned array. Default 2 (pairs)</param>
    /// <param name="indexOrderMatters">If turned true, [0, 1] and [1, 0] will be considered different and both return.</param>
    /// <param name="allowDuplicates">If turned true, [0, 0] and [1, 1] will be considered valid and be returned.</param>
    /// <returns>A list of arrays of length groupsOf, each containing the index pairs</returns>
    public static IEnumerable<int[]> IndexPairs<T>(this IEnumerable<T> array, int groupsOf = 2, bool indexOrderMatters = false, bool allowDuplicates = false)
    {
        return IndexPairs(array.Count(), groupsOf, indexOrderMatters);
    }
    /// <summary>
    /// Returns all pairs of indexes for the given length
    /// E.g. IndexPairs(3) => [0, 1], [0, 2], [1, 2]
    /// </summary>
    /// <param name="length">The number of indexes to find pairs for</param>
    /// <param name="groupsOf">The length of each returned array. Default 2 (pairs) </param>
    /// <param name="indexOrderMatters">If turned true, [0, 1] and [1, 0] will be considered different and both return.</param>
    /// <param name="allowDuplicates">If turned true, [0, 0] and [1, 1] will be considered valid and be returned.</param>
    /// <returns>A list of arrays of length groupsOf, each containing the index pairs</returns>
    public static IEnumerable<int[]> IndexPairs(int length, int groupsOf = 2, bool indexOrderMatters = false, bool allowDuplicates = false)
    {
        if (groupsOf > length) yield break;
        int[] array = Enumerable.Range(0, groupsOf).ToArray(); //start with enum range (e.g. 0, 1, 2..)
        if (allowDuplicates) Array.Fill(array, 0); //start from all 0s if duplicates are allowed
        int last = groupsOf - 1; //index of the last element of the array
        while (true)
        {
            if (!indexOrderMatters || allowDuplicates || array.All(new HashSet<int>().Add)) yield return array;
            array[last]++; //increase last element of the array
            while (array[last] == (indexOrderMatters || allowDuplicates ? length : length - groupsOf + last + 1)) //if that element is then bigger than is allowed...
            {
                if (last == 0) yield break; //if it's the leftmost element, we know we're done
                array[--last]++; //increment the one ahead of it instead.
            }
            if (indexOrderMatters && last < groupsOf - 1) array[++last] = 0;
            while (last < groupsOf - 1) //if we increased anything other than the last element...
            {
                array[last + 1] = array[last++] + (allowDuplicates ? 0 : 1); //...or to 1 more than the last one
            }
        }
    }
    /// <summary>
    /// Returns all pairs of values in the given array
    /// E.g. rgb.Pairs() => ["Red", "Green"], ["Red", "Blue"], ["Green", "Blue"]
    /// </summary>
    /// <typeparam name="T">The type of the passed array</typeparam>
    /// <param name="array">The array to iterate over and find pairs for</param>
    /// <param name="groupsOf">The size of each returned group, default 2 (pairs)</param>
    /// <param name="orderMatters">If turned true, [0, 1] and [1, 0] will be considered different and both return.</param>
    /// <param name="allowDuplicates">If turned true, [0, 0] and [1, 1] will be considered valid and be returned.</param>
    /// <returns>Each possible pair, as a list of arrays of length groupsOf</returns>
    public static IEnumerable<IEnumerable<T>> Pairs<T>(this IEnumerable<T> array, int groupsOf = 2, bool orderMatters = false, bool allowDuplicates = false)
    {
        foreach (int[] indexes in array.IndexPairs(groupsOf, orderMatters, allowDuplicates))
        {
            yield return indexes.Select(x => array.ElementAt(x));
        }
    }

}
