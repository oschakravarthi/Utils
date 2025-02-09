using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class TimeRangedLinkedList<T> : IEnumerable<T> where T : ITimeRanged
{
    private TimeRangedLinkedListNode first;

    private TimeRangedLinkedListNode last;

    public int Count { get; private set; }

    public T Latest => first == null ? default : first.Value;

    public T Oldest => last == null ? default : last.Value;

    public IEnumerator<T> GetEnumerator()
    {
        return new TimeRangedLinkedEnumerator(first, last);
    }

    //public void Purge(DateTime lessThan)
    //{
    //    while (last != null)
    //    {
    //        var upto = last.Value.GetUptoTimestamp();
    //        if (upto < lessThan)
    //        {
    //            last = last.Previous;
    //            if (last == null)
    //            {
    //                first = null;
    //            }
    //            else
    //            {
    //                last.Next = null;
    //            }
    //            count--;
    //        }
    //        else
    //        {
    //            //return;
    //        }
    //    }
    //}
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T value)
    {
        var node = new TimeRangedLinkedListNode
        {
            Value = value
        };
        if (first == null)
        {
            first = node;
            last = node;
        }
        else
        {
            node.Next = first;
            first.Previous = node;
            first = node;
        }

        Count++;
    }

    public void Add(IReadOnlyList<T> values)
    {
        for (var i = values.Count - 1; i >= 0; i--)
        {
            var value = values[i];
            Add(value);
        }
    }

    public void Append(TimeRangedLinkedList<T> list)
    {
        list.last.Next = first;
        first = list.first;
    }

    public IEnumerable<T> GetWindow(DateTime from, DateTime upto)
    {
        if (first == null)
        {
            yield break;
        }

        var p = first;
        while (p != null)
        {
            var value = p.Value;
            var itemFrom = value.GetFromTimestamp();
            if (itemFrom >= upto)
            {
                yield break;
            }

            var itemUpto = value.GetUptoTimestamp();
            if (itemUpto < from)
            {
                yield break;
            }

            yield return value;

            p = p.Next;
        }
    }

    public void Purge(DateTime lessThan)
    {
        var p = last;
        while (p != null)
        {
            var upto = p.Value.GetUptoTimestamp();
            if (upto < lessThan)
            //if ((from < upto && upto < lessThan) || (from == upto && from <= lessThan))
            {
                p = p.Previous;
                if (p == null)
                {
                    first = null;
                    last = null;
                    return;
                }

                Count--;
            }
            else
            {
                p.Next = null;
                last = p;
                return;
            }
        }
    }

    private class TimeRangedLinkedEnumerator(TimeRangedLinkedListNode from, TimeRangedLinkedListNode to) : IEnumerator<T>
    {
        private TimeRangedLinkedListNode p;
        private bool started;

        public T Current => p.Value;

        object IEnumerator.Current => p.Value;

        public void Dispose()
        {
            p = null;
        }

        public bool MoveNext()
        {
            if (!started)
            {
                p = from;
                started = true;
                return p != null;
            }

            if (p == null || p == to.Next)
            {
                return false;
            }

            p = p.Next;
            return p != null && p != to.Next;
        }

        public void Reset()
        {
            //p = list.first;
            p = null;
        }
    }

    private class TimeRangedLinkedListNode
    {
        public TimeRangedLinkedListNode Next { get; set; }
        public TimeRangedLinkedListNode Previous { get; set; }
        public T Value { get; set; }
    }
}