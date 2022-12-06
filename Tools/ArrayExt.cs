using System.Collections;
using System.ComponentModel;

public static class ArrayExt
{
    public static Random rng = new Random();
    public static void Shuffle<T>(this List<T> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            array.Swap(i, rng.Next(i));
        }
    }
    public static void Swap<T>(this List<T> array, params int[] locations) => array.Swap(locations[0], locations[1]);
    public static void Swap<T>(this List<T> array, int location1, int location2)
    {
        (array[location2], array[location1]) = (array[location1], array[location2]);
    }
    public static void Sort(this Array array) => Array.Sort(array);
    /// <summary>
    /// Reverses a portion of an array in the provided range. <br/>
    /// Reversed up to and NOT including the end index. <br/>
    /// <example>
    /// Example inputs and outputs using <c>[A,B,C,D,E]</c> as input;<br/>
    /// <code>
    /// abcde.Reverse(0..5)   => [E,D,C,B,A] (entirely reversed)
    /// abcde.Reverse(1..4)   => [A,D,C,B,E] (middle reversed)
    /// abcde.Reverse(i..i+1) => [A,B,C,D,E] (no change)
    /// abcde.Reverse(4..1)   => [A,D,C,B,E] (same as 1..4)
    /// abcde.Reverse(0..^2)  => [C,B,A,D,E] (first 3 reversed, last 2 ignored)
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="array">the array to reverse a segment of</param>
    /// <param name="r">the range to reverse</param>
    public static void Reverse(this Array array, Range r)
    {
        if (r.Start.GetOffset(array.Length) > r.End.GetOffset(array.Length)) r = new(r.End, r.Start);
        var (Offset, Length) = r.GetOffsetAndLength(array.Length);
        Array.Reverse(array, Offset, Length);
    }
    public static IEnumerable<T> ParseAs<T>(this IEnumerable<string> list, IFormatProvider? parseFormat = null) where T : IParsable<T>
    {
        if (typeof(string) == typeof(T) && list is IEnumerable<T> t) return t;
        return list.Select(s => T.Parse(s, parseFormat));
    }
    public static T ConvertTo<T>(this string item)
    {
        if (typeof(string) == typeof(T) && item is T t) return t;
        Func<string, T> parse = Utility.GetParse<T>();
        return parse(item);
    }
    public static IEnumerable<T> ConvertTo<T>(this IEnumerable<string> list)
    {
        if (typeof(string) == typeof(T) && list is IEnumerable<T> t) return t;
        //if (T is IParsable<T> tt) return tt.ParseAs<T>();
        Func<string, T> parse = Utility.GetParse<T>();
        return list.Select(parse).ToArray();
    }
    public static T[,] ConvertTo<T>(this object[,] array)
    {
        if (array is T[,] t) return t;
        T[,] converted = new T[array.GetLength(0), array.GetLength(1)];
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                object? ob = converter.ConvertFrom(array[i, j]);
                if (ob is T c) converted[i, j] = c;
            }
        }
        return converted;
    }
    #region Permutation searching
    /// <summary>
    /// Yields each possible permutation of a given collection. <br/>
    /// <example>
    /// <c>[A,B,C]</c> returns <c>[ [A,B,C], [A,C,B], [B,A,C], [B,C,A], [C,A,B], [C,B,A] ]</c>
    /// </example>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T[]> Permutations<T>(this IEnumerable<T> source)
    {
        foreach (T[] item in source.Pairs(source.Count(), orderMatters: true))
        {
            yield return item;
        }
    }
    /// <summary>
    /// Finds all possible combinations of array elements.<br/>
    /// <c>[A,B,C]</c> returns <c>[[],[A],[B],[C],[A,B],[A,C],[B,C],[A,B,C]]</c>
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
    /// Returns all pairs of indexes for the given length <br/>
    /// <example>
    /// E.g. (if array has length 3) <c>array.IndexPairs() => [0, 1], [0, 2], [1, 2]</c>
    /// </example>
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
    /// Returns all pairs of indexes for the given length<br/>
    /// The array returned is the same array each time<br/>
    /// <example>
    /// E.g. <c>IndexPairs(3) => [0, 1], [0, 2], [1, 2]</c>
    /// </example>
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
    /// Returns all pairs of values in the given array <br/>
    /// <example>
    /// E.g. <c>rgb.Pairs() => ["Red", "Green"], ["Red", "Blue"], ["Green", "Blue"]</c>
    /// </example>
    /// </summary>
    /// <typeparam name="T">The type of the passed array</typeparam>
    /// <param name="array">The array to iterate over and find pairs for</param>
    /// <param name="groupsOf">The size of each returned group, default 2 (pairs)</param>
    /// <param name="orderMatters">If turned true, [0, 1] and [1, 0] will be considered different and both return.</param>
    /// <param name="allowDuplicates">If turned true, [0, 0] and [1, 1] will be considered valid and be returned.</param>
    /// <returns>Each possible pair, as a list of arrays of length groupsOf</returns>
    public static IEnumerable<T[]> Pairs<T>(this IEnumerable<T> array, int groupsOf = 2, bool orderMatters = false, bool allowDuplicates = false)
    {
        foreach (int[] indexes in array.IndexPairs(groupsOf, orderMatters, allowDuplicates))
        {
            yield return indexes.Select(x => array.ElementAt(x)).ToArray();
        }
    }
    #endregion
    /// <summary>
    /// Same as .Select, but you also have access to the previous element in the array.<br/>
    /// Returned array will be 1 shorter than the input, as element 1 is skipped.
    /// <example>
    /// <code>
    /// array.SelectWithPrevious((prev, curr) => prev + curr);
    /// </code>
    /// If <c>[1, 2, 3, 4]</c> was passed in with the above example, would return <c>[3, 5, 7]</c>
    /// </example>
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
    public static IEnumerable<T> Overlap<T>(this IEnumerable<T> a, IEnumerable<T> b, bool orderMatters = true)
    {
        if (!orderMatters) return a.Intersect(b);
        if (a.Count() > b.Count()) Utility.RefSwap(ref a, ref b);
        return a.Where((e, i) => Equals(e,b.ElementAt(i))).ToArray();
    }
    /// <summary>
    /// A function to retrive the most common element in a collection <br/>
    /// <example>
    /// For example, <c>"ACDC".Mode()</c> returns the tuple <c>(C, 2)</c>
    /// </example>
    /// </summary>
    /// <typeparam name="T">The type of the collection</typeparam>
    /// <returns>A tuple of the most common element and the count of appearances</returns>
    public static (T? item, int count) Mode<T>(this IEnumerable<T> collection) => collection.Mode(t => t);
    /// <summary>
    /// A function to retrive the most common element in a collection <br/>
    /// <example>
    /// For example, <c>"ACDC".Mode()</c> returns the tuple <c>(C, 2)</c>
    /// </example>
    /// </summary>
    /// <typeparam name="T">The type of the collection</typeparam>
    /// <param name="groupBy">A function that extracts the value to group segments by</param>
    /// <returns>A tuple of the most common element and the count of appearances</returns>
    public static (TKey? item, int count) Mode<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> groupBy)
    {
        IGrouping<TKey, T>? group = collection.GroupBy(groupBy).MaxBy(g => g.Count());
        if (group == null) return (default, 0);
        return (group.Key, group.Count());
    }
    public static IEnumerable<int[]> SegmentsSplitIndexes<T>(this IEnumerable<T> a, int segmentCount) => SegmentsSplitIndexes(a.Count(), segmentCount);
    public static IEnumerable<int[]> SegmentsSplitIndexes(int length, int segmentCount)
    {
        foreach (int[] item in IndexPairs(length + 1, segmentCount - 1, allowDuplicates: true))
        {
            yield return item.Append(length).Prepend(0).ToArray();
        }
    }
    public static IEnumerable<IEnumerable<int>> SplitToChunkLengths<T>(this IEnumerable<T> collection, int segmentCount)
    {
        return collection.Count().SplitToChunks(segmentCount);
    }
    public static IEnumerable<IEnumerable<int>> SplitToChunks(this int number, int chunkCount)
    {
        foreach (var indexes in SegmentsSplitIndexes(number, chunkCount))
        {
            yield return indexes.SelectWithPrevious((prev, curr) => curr - prev);
        }
    }
    public static IEnumerable<IEnumerable<T>> SplitAtIndexes<T>(this IEnumerable<T> collection, params int[] splitIndexes)
    {
        yield return collection.Take(splitIndexes[0]);
        for (int i = 0; i < splitIndexes.Length - 1; i++)
        {
            yield return collection.Skip(splitIndexes[i]).Take(splitIndexes[i+1] - splitIndexes[i]);
        }
        yield return collection.Skip(splitIndexes[^1]);
    }
    public static IEnumerable<IEnumerable<IEnumerable<T>>> SplitToChunks<T>(this IEnumerable<T> array, int segmentCount)
    {
        foreach (int[] indexes in SegmentsSplitIndexes(array.Count(),segmentCount))
        {
            yield return indexes.SelectWithPrevious((prev, curr) => array.Skip(prev).Take(curr - prev));
        }
    }
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> collection)
    {
        return collection.Select((item, index) => (item, index));
    }
    public static T[][] Transpose<T>(this T[][] array)
    {
        int rowCount = array.Length, columnCount = array[0].Length;
        if (array.Any(a => a.Length != columnCount)) throw new ArgumentException("Transpose only accepts jagged arrays with an equal count of items inside each inner array");
        T[][] transposed = new T[columnCount][];
        for (int column = 0; column < columnCount; column++)
        {
            transposed[column] = new T[rowCount];
            for (int row = 0; row < rowCount; row++)
            {
                transposed[column][row] = array[row][column];
            }
        }
        return transposed;
    }
    public static T[,] Transpose<T>(this T[,] array)
    {
        int rowCount = array.GetLength(0), columnCount = array.GetLength(1);
        T[,] transposed = new T[columnCount, rowCount];
        for (int column = 0; column < columnCount; column++)
        {
            for (int row = 0; row < rowCount; row++)
            {
                transposed[column, row] = array[row, column];
            }
        }
        return transposed;
    }
    public static int NestedDepth(this IEnumerable array)
    {
        IEnumerator enumerator = array.GetEnumerator();
        if (!enumerator.MoveNext()) return 1;
        if (enumerator.Current is IEnumerable e) return 1 + e.NestedDepth();
        return 1;
    }
    public static T[][] ToJagged<T>(this T[,] array2D)
    {
        int rows = array2D.GetUpperBound(0);
        int columns = array2D.GetUpperBound(1);
        T[][] jaggedArray = new T[rows + 1][];
        for (int i = array2D.GetLowerBound(0); i <= rows; i++)
        {
            jaggedArray[i] = new T[columns + 1];

            for (int j = array2D.GetLowerBound(1); j <= columns; j++)
            {
                jaggedArray[i][j] = array2D[i, j];
            }
        }
        return jaggedArray;
    }
    public static string ToContentString<T>(this T[,] array2D, string separator = ", ")
    {
        return array2D.ToJagged().ToContentString(separator);
    }
    public static string ToContentString(this IEnumerable array, string separator = ", ")
    {
        return array.ToContentString(array.NestedDepth(), array.NestedDepth(), separator);
    }
    private static string ToContentString(this IEnumerable array, int maxDepth, int depth, string separator)
    {
        string content = "{";
        bool first = true;
        foreach (var item in array)
        {
            if (!first) content += separator;
            if (depth > 1) content += "\n";
            if (item is IEnumerable e)
            {
                if (depth > 1) content += "¦ ".Repeat(1 + maxDepth - depth);
                content += $"{e.ToContentString(maxDepth, depth - 1, separator)}";
            }
            else
            {
                content += item.ToString();
            }
            first = false;
        }
        if (depth > 1) content += "\n" + "¦ ".Repeat(maxDepth - depth);
        return content + "}";
    }
    public static int Mul(this IEnumerable<int> array) => array.Aggregate(1, (a, b) => a * b);
    public static long Mul(this IEnumerable<long> array) => array.Aggregate<long, long>(1, (a, b) => a * b);
    public static long MulAsLong(this IEnumerable<int> array) => array.Select(i => (long)i).Mul();
    public static IEnumerable<T> Shortest<T>(this IEnumerable<IEnumerable<T>> array) => array.MinBy(x => x.Count());
    public static IEnumerable<T> Longest<T>(this IEnumerable<IEnumerable<T>> array) => array.MaxBy(x => x.Count());
    public static IEnumerable<int> Counts<T>(this IEnumerable<IEnumerable<T>> array) => array.Select(x => x.Count());
    public static int FirstIndex<T>(this T[] array, Func<T, bool> predicate, out T item)
    {
        item = default;
        IEnumerable<T> items = array.Where(predicate);
        if (!items.Any()) return -1;
        item = items.First();
        return Array.IndexOf(array, item);
    }
    public static int FirstIndex(this string s, Func<char, bool> predicate, out char item)
    {
        item = default;
        IEnumerable<char> chars = s.Where(predicate);
        if (!chars.Any()) return -1;
        item = chars.First();
        return s.IndexOf(item);
    }
    public static TResult[,] Select2D<TSource, TResult>(this TSource[,] array, Func<TSource, TResult> predicate)
    {
        TResult[,] result = new TResult[array.GetLength(0), array.GetLength(1)];
        for (int x = array.GetLowerBound(0); x < array.GetUpperBound(0); x++)
        {
            for (int y = array.GetLowerBound(1); y < array.GetUpperBound(1); y++)
            {
                result[x, y] = predicate.Invoke(array[x, y]);
            }
        }
        return result;
    }
    public static IEnumerable<T> Flatten<T>(this T[,] array)
    {
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                yield return array[x, y];
            }
        }
    }
    public static IEnumerable<T> AggregateSteps<T>(this IEnumerable<T> source, Func<T, T, T> func)
    {
        using IEnumerator<T> e = source.GetEnumerator();
        if (!e.MoveNext()) throw new IndexOutOfRangeException("No elements");
        T result = e.Current;
        yield return result;
        while (e.MoveNext())
        {
            result = func(result, e.Current);
            yield return result;
        }
    }
    public static IEnumerable<TSeed> AggregateSteps<T, TSeed>(this IEnumerable<T> source, TSeed seed, Func<TSeed, T, TSeed> func)
    {
        using IEnumerator<T> e = source.GetEnumerator();
        if (!e.MoveNext()) throw new IndexOutOfRangeException("No elements");
        TSeed result = seed;
        foreach (var item in source)
        {
            seed = func(seed, item);
            yield return seed;
        }
    }
    public static IEnumerable<IGrouping<T, T>> Group<T>(this IEnumerable<T> source) => source.GroupBy(x => x);
    public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> source)
    {
        HashSet<T> seen = new();
        foreach (var item in source)
        {
            if (seen.Contains(item)) yield return item;
            seen.Add(item);
        }
    }
    public static IEnumerable<T> Loop<T>(this IEnumerable<T> source, int times)
    {
        for (int i = 0; i < times; i++)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }
    }
    public static IEnumerable<T> LoopForever<T>(this IEnumerable<T> source)
    {
        while (true)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }
    }
    public static IEnumerable<T> Without<T>(this IEnumerable<T> source, T unwanted) where T : notnull
    {
        return source.Where(s => !s.Equals(unwanted));
    }

    //future update; work with any INumber
    public static void AddToRange(this int[] source, Range range, int amount = 1)
    {
        (int Offset, int Length) = range.GetOffsetAndLength(source.Length);
        for (int i = Offset; i < Offset + Length; i++)
        {
            source[i] += amount;
        }
    }
    public static void AddToRange(this int[,] source, Range Yrange, Range Xrange, int amount = 1)
    {
        (int XOffset, int XLength) = Xrange.GetOffsetAndLength(source.Length);
        (int YOffset, int YLength) = Yrange.GetOffsetAndLength(source.Length);
        for (int x = XOffset; x < XOffset + XLength; x++)
        {
            for (int y = YOffset; y < YOffset + YLength; y++)
            {
                source[x, y] += amount;
            }
        }
    }
    //for linked lists
    public static LinkedListNode<T> GetNodeAt<T>(this LinkedList<T> list, int position)
    {
        LinkedListNode<T> mark = list.First;
        while (position-- > 0) mark = mark.Next;
        return mark;
    }
}
