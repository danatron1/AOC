using System.Numerics;

namespace AOC.Interfaces;

public interface IPoint<T> : IEquatable<T>, IEqualityComparer<T>,
    IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>,
    IMultiplyOperators<T, int, T>, IDivisionOperators<T, int, T>, IComparable<T>,
    IPathfinderNode<T>
    where T : IPoint<T>
{
    static abstract T Zero { get; }
    static abstract T One { get; }
    public int ManhattanDistanceTo(T other);
    public double EuclideanDistanceTo(T other);
}

public interface IPositional<T> where T : IPoint<T>
{
    public T Point { get; set; }
    public int ManhattanDistanceTo(IPositional<T> other);
    public double EuclideanDistanceTo(IPositional<T> other);
}