using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class DateTimeSortedList<T> : ICollection<Timestamped<T>>
{
    private readonly List<Timestamped<T>> list = [];

    public DateTime From => list[0].Timestamp;

    public DateTime Upto => list[list.Count - 1].Timestamp;

    public Timestamped<T> this[int index] => list[index];

    public int Count => list.Count;

    public bool IsReadOnly => false;

    public void Add(Timestamped<T> item)
    {
        list.Add(item);
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool Contains(Timestamped<T> item)
    {
        return list.Contains(item);
    }

    public void CopyTo(Timestamped<T>[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Timestamped<T>> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public bool Remove(Timestamped<T> item)
    {
        return list.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddRange(IEnumerable<Timestamped<T>> items)
    {
        list.AddRange(items);
    }

    public DateTimeRanged<T> GetContained(DateTime datetime)
    {
        var floor = GetFloor(datetime);
        if (floor < 0)
        {
            return null;
        }

        if (floor + 1 > list.Count - 1)
        {
            return null;
        }

        return new DateTimeRanged<T>(list[floor].Timestamp, list[floor + 1].Timestamp, list[floor].Info);
    }

    public int GetFloor(DateTime datetime, Func<T, bool> selector)
    {
        var index = GetFloor(datetime);
        if (selector != null)
        {
            if (index > -1)
            {
                for (; index >= 0; index--)
                    if (selector(list[index].Info))
                    {
                        break;
                    }
            }
        }

        return index;
    }

    public int GetFloor(DateTime datetime)
    {
        if (Count == 0)
        {
            return -1;
        }

        if (datetime <= this[0].Timestamp)
        {
            return 0;
        }

        var index = -1;
        var left = 0;
        var right = list.Count - 1;
        while (right >= left)
        {
            var mid = (left + right) / 2;
            var item = list[mid];
            if (item.Timestamp == datetime)
            {
                index = mid;
                break;
            }

            if (item.Timestamp < datetime)
            {
                if (mid + 1 < list.Count)
                {
                    if (list[mid + 1].Timestamp > datetime)
                    {
                        index = mid;
                        break;
                    }

                    left = mid + 1;
                }
                else
                {
                    break;
                }
            }
            else
            {
                right = mid - 1;
            }
        }

        return index;
    }

    public IEnumerable<Timestamped<T>> GetItemsWithinRange(Range<DateTime> range)
    {
        var from = GetCeiling(range.From);
        if (from >= 0)
        {
            var upto = GetFloor(range.Upto);
            if (upto >= 0)
            {
                for (var i = from; i <= upto; i++)
                    yield return this[i];
            }
        }
    }

    public IEnumerable<DateTimeRanged<T>> GetOverlapped(Range<DateTime> range)
    {
        return GetOverlapped(range.From, range.Upto);
    }

    public IEnumerable<DateTimeRanged<T>> GetOverlapped(DateTime from, DateTime upto)
    {
        return GetOverlapped(from, upto, null);
    }

    public IEnumerable<DateTimeRanged<T>> GetOverlapped(Range<DateTime> range, Func<T, bool> selector)
    {
        return GetOverlapped(range.From, range.Upto, selector);
    }

    public IEnumerable<DateTimeRanged<T>> GetOverlapped(DateTime from, DateTime upto, Func<T, bool> selector)
    {
        return GetOverlapped(from, upto, selector, (f, u, i) => new DateTimeRanged<T>(f, u, i));
    }

    //public IEnumerable<E> GetOverlapped<E>(DateTime from, DateTime upto, Func<T, bool> selector, Func<DateTime, DateTime, T, E> entityFunc)
    //{
    //    var items = GetItems(from, upto, selector);
    //    Timestamped<T> first = null;
    //    foreach (var item in items)
    //    {
    //        if (first == null)
    //        {
    //            first = item;
    //        }
    //        else
    //        {
    //            yield return entityFunc(first.Timestamp, item.Timestamp, first.Info);
    //            first = item;
    //        }
    //    }
    //}
    public IEnumerable<E> GetOverlapped<E>(DateTime from, DateTime upto, Func<T, bool> selector,
        Func<DateTime, DateTime, T, E> entityFunc)
    {
        if (Count == 0)
        {
            yield break;
        }

        if (this[0].Timestamp > upto)
        {
            yield break;
        }

        var fromIndex = GetFloor(from, selector);
        var toIndex = GetCeiling(upto, selector);
        if (fromIndex < 0)
        {
            fromIndex = 0;
        }

        if (toIndex < 0)
        {
            toIndex = list.Count - 1;
        }

        if (fromIndex >= 0 && toIndex >= 0 && toIndex < list.Count)
        {
            Timestamped<T> first = null;
            for (var i = fromIndex; i <= toIndex; i++)
            {
                var item = list[i];
                if (selector != null && !selector(item.Info))
                {
                    continue;
                }

                if (first == null)
                {
                    first = item;
                }
                else
                {
                    yield return entityFunc(first.Timestamp, item.Timestamp, first.Info);
                    first = item;
                }
            }
        }
    }

    public IEnumerable<DateTimeRanged<T>> GetRanged()
    {
        for (var i = 1; i < list.Count; i++)
        {
            var previous = list[i - 1];
            yield return new DateTimeRanged<T>(previous.Timestamp, list[i].Timestamp, previous.Info);
        }
    }

    public IEnumerable<E> GetRanged<E>(Func<DateTime, DateTime, T, E> entityFunc)
    {
        for (var i = 1; i < list.Count; i++)
        {
            var previous = list[i - 1];
            var entity = entityFunc(previous.Timestamp, list[i].Timestamp, previous.Info);
            yield return entity;
        }
    }

    public int IndexOf(Timestamped<T> item)
    {
        return list.IndexOf(item);
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }

    public void Sort()
    {
        list.Sort(TimestampedComparer<ITimestamped>.Instance);
    }

    public override string ToString()
    {
        if (list.Count < 2)
        {
            return base.ToString();
        }

        var from = list[0].Timestamp;
        var upto = list[list.Count - 1].Timestamp;
        return $"{from} - {upto}";
    }

    private int GetCeiling(DateTime datetime, Func<T, bool> selector)
    {
        var index = GetCeiling(datetime);
        if (selector != null)
        {
            if (index > -1 && index < list.Count)
            {
                for (; index < list.Count; index++)
                    if (selector(list[index].Info))
                    {
                        break;
                    }
            }
        }

        return index;
    }

    private int GetCeiling(DateTime datetime)
    {
        if (Count == 0)
        {
            return -1;
        }

        if (datetime <= this[0].Timestamp)
        {
            return 0;
        }

        if (datetime >= this[Count - 1].Timestamp)
        {
            return Count;
        }

        var index = -1;
        var left = 0;
        var right = list.Count - 1;
        while (right >= left)
        {
            var mid = (left + right) / 2;
            var item = list[mid];
            if (item.Timestamp == datetime)
            {
                index = mid;
                break;
            }

            if (item.Timestamp > datetime)
            {
                if (mid - 1 > -1)
                {
                    if (list[mid - 1].Timestamp < datetime)
                    {
                        index = mid;
                        break;
                    }

                    right = mid - 1;
                }
                else
                {
                    break;
                }
            }
            else
            {
                left = mid + 1;
            }
        }

        return index;
    }
}