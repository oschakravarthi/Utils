using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public static class Extensions
{
    public static IEnumerable<DateTimeRanged<T>> GetRanged<T>(this IEnumerable<Timestamped<T>> items)
    {
        Timestamped<T> previous = null;
        foreach (var item in items)
        {
            if (previous == null)
            {
                previous = item;
                continue;
            }

            yield return new DateTimeRanged<T>(previous.Timestamp, item.Timestamp, previous.Info);
            previous = item;
        }
    }

    public static IEnumerable<DateTimeRanged<T>> GetRangedWithNoDuplicates<T>(this IEnumerable<Timestamped<T>> items)
    {
        return GetRangedWithNoDuplicates(items, (x, y) => x.Equals(y));
    }

    public static IEnumerable<DateTimeRanged<T>> GetRangedWithNoDuplicates<T>(this IEnumerable<Timestamped<T>> items, Func<T, T, bool> equalityComparer)
    {
        Timestamped<T> previous = null;
        Timestamped<T> last = null;

        foreach (var item in items)
        {
            last = item;
            if (previous == null)
            {
                previous = item;
                continue;
            }
            if (!equalityComparer(previous.Info, item.Info))
            {
                yield return new DateTimeRanged<T>(previous.Timestamp, item.Timestamp, previous.Info);
                previous = item;
            }
        }
        //if (last != previous)
        //{
        //    yield return new DateTimeRanged<T>(previous.Timestamp, last.Timestamp, previous.Info);
        //}
        if (previous != null)
        {
            yield return new DateTimeRanged<T>(previous.Timestamp, DateTime.Now, previous.Info);
        }
    }

    public static IEnumerable<E> GetRanged<T, E>(this IEnumerable<Timestamped<T>> items,
        Func<DateTime, DateTime, T, E> entityFunc)
    {
        Timestamped<T> previous = null;
        foreach (var item in items)
        {
            if (previous == null)
            {
                previous = item;
                continue;
            }

            yield return entityFunc(previous.Timestamp, item.Timestamp, previous.Info);
            previous = item;
        } //Func<DateTime, DateTime, T, E> entityFunc
    }
}