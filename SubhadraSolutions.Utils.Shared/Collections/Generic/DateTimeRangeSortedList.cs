using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class DateTimeRangeSortedList<T> : ICollection<T> where T : DateTimeRange
{
    private readonly List<T> list = [];

    public DateTimeRangeSortedList()
    {
    }

    public DateTimeRangeSortedList(IEnumerable<T> items)
    {
        AddRange(items);
    }

    public T this[int index] => list[index];

    public int Count => list.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        list.Add(item);
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0)
        {
            return false;
        }

        list.RemoveAt(index);
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddRange(IEnumerable<T> items)
    {
        list.AddRange(items);
    }

    public T GetContained(DateTime datetime)
    {
        var index = Search(datetime);
        if (index > -1)
        {
            var item = list[index];
            if (item.Contains(datetime))
            {
                return item;
            }
        }

        return null;
    }

    //public IEnumerable<T> GetContained(Range<DateTime> range)
    //{
    //    foreach (var item in list)
    //    {
    //        if (item.Contains(range))
    //        {
    //            yield return item;
    //        }
    //    }
    //}
    public IEnumerable<T> GetContained(Range<DateTime> range)
    {
        var index1 = Search(range.From);
        if (index1 < 0)
        {
            index1 = 0;
        }
        else
        {
            index1++;
        }

        var index2 = Search(range.Upto);
        if (index2 == index1)
        {
            index2--;
        }

        for (var i = index1; i <= index2; i++) yield return list[i];
    }

    public IEnumerable<T> GetOverlapped(Range<DateTime> range)
    {
        //foreach (var item in list)
        //{
        //    if (item.OverlapsWith(range))
        //    {
        //        yield return item;
        //    }
        //}

        var index1 = Search(range.From);
        if (index1 < 0)
        {
            index1 = 0;
        }

        var index2 = Search(range.Upto);
        if (index2 < 0)
        {
            index2 = Count - 1;
        }

        for (var i = index1; i <= index2; i++)
            if (list[i].OverlapsWith(range))
            {
                yield return list[i];
            }
    }

    public int IndexOf(T item)
    {
        return list.BinarySearch(item);
    }

    public int Search(DateTime datetime)
    {
        return Search(datetime, x => x.From);
    }

    public int Search(DateTime datetime, Func<T, DateTime> func)
    {
        var low = 0;
        var high = list.Count - 1;
        while (true)
        {
            // If low and high cross each other
            if (low > high)
            {
                return -1;
            }

            // If last element is smaller than x
            if (datetime >= func(list[high]))
            {
                return high;
            }

            // Find the middle point
            var mid = (low + high) / 2;

            var item = list[mid];

            // If middle point is floor.
            if (func(item) == datetime)
            {
                var temp = mid;
                while (temp > 0 && func(list[temp - 1]) == datetime) temp--;
                return temp;
            }

            // If x lies between mid-1 and mid
            if (mid > 0 && func(list[mid - 1]) <= datetime && datetime < func(item))
            {
                return mid - 1;
            }

            // If x is smaller than mid, floor
            // must be in left half.
            if (datetime < func(item))
            {
                high = mid - 1;
            }
            else
            // If mid-1 is not floor and x is
            // greater than arr[mid],
            {
                low = mid + 1;
            }
        }
    }

    public void Sort()
    {
        list.Sort(DateTimeRangeComparer<T>.Instance);
    }

    public override string ToString()
    {
        if (list.Count == 0)
        {
            return base.ToString();
        }

        var from = list[0].From;
        var upto = list[list.Count - 1].Upto;
        return $"{from} - {upto}";
    }
}