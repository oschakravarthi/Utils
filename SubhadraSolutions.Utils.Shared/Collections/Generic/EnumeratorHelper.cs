using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public static class EnumeratorHelper
{
    public static IEnumerable<T> Merge<T>(IEnumerable<T> a, IEnumerable<T> b, bool removeDuplicates = false)
        where T : IComparable<T>
    {
        return Merge(a.GetEnumerator(), b.GetEnumerator(), removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerable<T> a, IEnumerable<T> b, IComparer<T> comparer,
        bool removeDuplicates = false)
    {
        return Merge(a.GetEnumerator(), b.GetEnumerator(), comparer, removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerable<T> a, IEnumerable<T> b, Comparison<T> comparer,
        bool removeDuplicates = false)
    {
        return Merge(a.GetEnumerator(), b.GetEnumerator(), comparer, removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerator<T> a, IEnumerator<T> b, bool removeDuplicates = false)
        where T : IComparable<T>
    {
        return Merge(a, b, (x, y) => x.CompareTo(y), removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerator<T> a, IEnumerator<T> b, IComparer<T> comparer,
        bool removeDuplicates = false)
    {
        return Merge(a, b, comparer.Compare, removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerator<T> a, IEnumerator<T> b, Comparison<T> comparer,
        bool removeDuplicates = false)
    {
        var aCanMove = a.MoveNext();
        var bCanMove = b.MoveNext();

        while (aCanMove || bCanMove)
        {
            if (aCanMove && bCanMove)
            {
                var compareResult = comparer(a.Current, b.Current);
                if (compareResult == 0)
                {
                    yield return a.Current;
                    if (!removeDuplicates)
                    {
                        yield return b.Current;
                    }

                    aCanMove = a.MoveNext();
                    bCanMove = b.MoveNext();
                    continue;
                }

                if (compareResult < 0)
                {
                    yield return a.Current;
                    aCanMove = a.MoveNext();
                }
                else
                {
                    yield return b.Current;
                    bCanMove = b.MoveNext();
                }

                continue;
            }

            if (aCanMove)
            {
                while (aCanMove)
                {
                    yield return a.Current;
                    aCanMove = a.MoveNext();
                }
            }
            else
            {
                while (bCanMove)
                {
                    yield return b.Current;
                    bCanMove = b.MoveNext();
                }
            }
        }
    }
}