using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tally<T> : IEnumerable<T> where T : notnull
{
    readonly Dictionary<T, long> dictionary;
    Random rng;
    private bool _canContainNegative;
    public long Total => Sum();
    public long TotalOfPositive => SumBetween(1);
    public List<T> Values => GetValues();
    public long Count => CountUnique();
    public bool CanContainNegative
    {
        get { return _canContainNegative; }
        set
        {
            _canContainNegative = value;
            if (!value) RemoveNegativeValues();
        }
    }
    public T Maximum => dictionary.MaxBy(v => v.Value).Key;
    public T Minimum => dictionary.MinBy(v => v.Value).Key;
    public long this[T item]
    {
        get
        {
            return CountOf(item);
        }
        set
        {
            Set(item, value);
        }
    }
    public Tally(bool allowNegatives = true)
    {
        dictionary = new Dictionary<T, long>();
        rng = new Random();
        CanContainNegative = allowNegatives;
    }
    private long RandomLong(long max = long.MaxValue, long min = 0)
    {
        if (max <= int.MaxValue) return rng.Next((int)max);
        long result = rng.Next((int)(min >> 32), (int)(max >> 32));
        result <<= 32;
        result = result | (long)rng.Next((int)min, (int)max);
        return result;
    }
    public T GetWeightedRandom()
    {
        long selection = RandomLong(TotalOfPositive);
        long sum = 0;
        foreach (T item in this)
        {
            if (this[item] < 0) continue;
            sum += this[item];
            if (sum >= selection) return item;
        }
        return default(T);
    }
    public bool Add(KeyValuePair<T, int> item) => Add(item.Key, item.Value);
    public bool Add(T item, long amount = 1)
    {
        if (amount == 0) return false;
        if (dictionary.ContainsKey(item)) dictionary[item] += amount;
        else dictionary[item] = amount;
        RemoveIfNecessary(item);
        return true;
    }
    public void AddRange(Tally<T> collection)
    {
        if (collection is null) throw new ArgumentNullException();
        foreach (var item in collection) Add(item, collection[item]);
    }
    public bool AddRange(long amount, params T[] items)
    {
        if (items == null || items.Length == 0) return false;
        foreach (T item in items) Add(item, amount);
        return true;
    }
    public bool AddRange(params T[] items) => AddRange(1, items);
    public bool AddRange(IEnumerable<T> items) => AddRange(1, items.ToArray());
    public bool AddRange(IEnumerable<KeyValuePair<T, int>> items)
    {
        if (items is null || items.Count() == 0) return false;
        foreach (var item in items) Add(item);
        return true;
    }
    public void SubtractRange(Tally<T> collection)
    {
        if (collection is null) throw new ArgumentNullException();
        foreach (var item in collection) Subtract(item, collection[item]);
    }
    public bool SubtractRange(long amount, params T[] items)
    {
        if (items is null || items.Length == 0) return false;
        foreach (T item in items) Subtract(item, amount);
        return true;
    }
    public bool SubtractRange(params T[] items) => SubtractRange(1, items);
    public void Set(T item, long amount)
    {
        dictionary[item] = amount;
        RemoveIfNecessary(item);
    }
    private void RemoveIfNecessary(T item)
    {
        if (dictionary[item] == 0 || (!CanContainNegative && dictionary[item] < 0)) Remove(item);
    }
    public long Sum() => dictionary.Values.Sum();
    public long SumBetween(long min = long.MinValue, long max = long.MaxValue) => dictionary.Values.Where(i => i >= min && i <= max).Sum();
    public long CountAll(long min = long.MinValue, long max = long.MaxValue) => dictionary.Values.Count(i => i >= min && i <= max);
    public long CountUnique() => dictionary.Count();
    public long CountOf(T item) => dictionary.TryGetValue(item, out long count) ? count : 0;
    public bool Contains(T item, long minimumAmount = 1) => CountOf(item) >= minimumAmount;
    public bool Remove(T item) => dictionary.Remove(item);
    public bool Subtract(T item) => Subtract(item, 1);
    public bool Subtract(T item, long amount) => Add(item, -amount);
    public long Max() => dictionary.Values.Max();
    public long Min() => dictionary.Values.Min();
    public bool Take(T item, long amount = 1)
    {
        if (!Contains(item, amount)) return false;
        else Subtract(item, amount);
        return true;
    }
    public void Clear() => dictionary.Clear();
    public void RemoveNegativeValues()
    {
        foreach (T item in dictionary.Keys)
        {
            if (dictionary[item] <= 0) Remove(item);
        }
    }
    public List<T> GetValues() => dictionary.Keys.ToList();
    public Type GetItemType() => typeof(T);
    public HashSet<T> GetHashSet() => dictionary.Keys.ToHashSet();
    public Dictionary<T, long> ToDictionary() => dictionary;
    public IEnumerable<KeyValuePair<T, long>> ToKeyValuePairs() => dictionary;
    public void MergeWith(Tally<T> otherTally)
    {
        foreach (T item in otherTally.GetValues())
        {
            Add(item, otherTally.CountOf(item));
        }
    }
    public static Tally<T> operator +(Tally<T> a, Tally<T> b)
    {
        Tally<T> newTally = a.Clone();
        newTally.AddRange(b);
        return newTally;
    }
    public static Tally<T> operator -(Tally<T> a, Tally<T> b)
    {
        Tally<T> newTally = a.Clone();
        newTally.SubtractRange(b);
        return newTally;
    }
    public Tally<T> Clone()
    {
        Tally<T> copy = new(CanContainNegative);
        copy.MergeWith(this);
        return copy;
    }
    public IEnumerator<T> GetEnumerator()
    {
        return dictionary.Keys.OrderBy(item => dictionary[item]).Reverse().GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public static class TallyExtensions
{
    public static Tally<T> ToTally<T>(this IEnumerable<KeyValuePair<T, int>> source) where T : notnull
    {
        Tally<T> tally = new();
        tally.AddRange(source);
        return tally;
    }
    public static Tally<T> ToTally<T>(this IEnumerable<T> source) where T : notnull
    {
        Tally<T> tally = new();
        tally.AddRange(source.ToArray());
        return tally;
    }

    public static Tally<T> Frequencies<T>(this IEnumerable<T> collection) where T : notnull
    {
        Tally<T> t = new();
        t.AddRange(collection);
        return t;
    }
}