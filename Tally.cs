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
    public bool Add(T item, long amount = 1)
    {
        if (amount == 0) return false;
        if (dictionary.ContainsKey(item)) dictionary[item] += amount;
        else dictionary[item] = amount;
        RemoveIfNecessary(item);
        return true;
    }
    public bool AddRange(long amount, params T[] items)
    {
        if (items == null || items.Length == 0) return false;
        foreach (T item in items)
        {
            Add(item, amount);
        }
        return true;
    }
    public bool AddRange(params T[] items) => AddRange(1, items);
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
    public bool Remove(T item, long amount) => Add(item, -amount);
    public long Max() => dictionary.Values.Max();
    public long Min() => dictionary.Values.Min();
    public bool Take(T item, long amount = 1)
    {
        if (!Contains(item, amount)) return false;
        else Remove(item, amount);
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
    public Dictionary<T, long> GetDictionary() => dictionary;
    public void MergeWith(Tally<T> otherTally)
    {
        foreach (T item in otherTally.GetValues())
        {
            Add(item, otherTally.CountOf(item));
        }
    }
    public Tally<T> Clone()
    {
        Tally<T> copy = new Tally<T>();
        copy.MergeWith(this);
        return copy;
    }
    public IEnumerator<T> GetEnumerator()
    {
        return dictionary.Keys.OrderBy(item => dictionary[item]).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
