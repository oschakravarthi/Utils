using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class Range<T> : IComparable<Range<T>> where T : IComparable<T>
{
    public Range(Range<T> range)
    {
        From = range.From;
        Upto = range.Upto;
    }

    public Range(T from, T upto)
    {
        From = from;
        Upto = upto;
    }

    [JsonInclude] public T From { get; private set; }

    public bool IsZero => From.CompareTo(Upto) == 0;

    [JsonInclude] public T Upto { get; private set; }

    public int CompareTo(Range<T> other)
    {
        var result = From.CompareTo(other.From);
        if (result == 0)
        {
            result = Upto.CompareTo(other.Upto);
        }

        return result;
    }

    public bool Contains(T item)
    {
        return item.CompareTo(From) >= 0 && item.CompareTo(Upto) < 0;
    }

    public bool Contains(Range<T> range)
    {
        return range.From.CompareTo(From) >= 0 && range.Upto.CompareTo(Upto) < 0;
    }

    public List<Range<T>> Exclude(IEnumerable<Range<T>> others)
    {
        var result = new Queue<Range<T>>();
        result.Enqueue(this);

        foreach (var toExclude in others)
        {
            var count = result.Count;
            for (var i = 0; i < count; i++)
            {
                var input = result.Dequeue();
                if (toExclude.OverlapsWith(input))
                {
                    if (input.From.CompareTo(toExclude.From) < 0)
                    {
                        result.Enqueue(new Range<T>(input.From, toExclude.From));
                    }

                    if (input.Upto.CompareTo(toExclude.Upto) > 0)
                    {
                        result.Enqueue(new Range<T>(toExclude.Upto, input.Upto));
                    }
                }
                else
                {
                    result.Enqueue(input);
                }
            }
        }

        return result.ToList();
    }

    public Range<T> GetIntersection(Range<T> other)
    {
        var a = this;
        var b = other;
        if (a.From.CompareTo(b.From) > 0)
        {
            (a, b) = (b, a);
        }

        if (a.Upto.CompareTo(b.From) <= 0)
        {
            return null;
        }

        if(a.Upto.CompareTo(b.Upto) > 0)
        {
            return b;
        }
        return new Range<T>(b.From, a.Upto);
    }
    public static List<Range<T>> Merge(IEnumerable<Range<T>> ranges)
    {
        var input = ranges.ToList();
        if (input.Count == 0)
        {
            return new List<Range<T>>(0);
        }
        input.Sort((x, y) => x.From.CompareTo(y.From));
        var result = new List<Range<T>>();
        var previous = input[0];
        for (int i = 1; i < input.Count; i++)
        {
            var current = input[i];
            if (previous.Upto.CompareTo(current.From) >= 0)
            {
                previous = new Range<T>(previous.From, current.Upto);
            }
            else
            {
                result.Add(previous);
                previous = current;
            }
        }
        result.Add(previous);
        return result;
    }
    public bool OverlapsWith(Range<T> other)
    {
        if (IsZero || other.IsZero)
        {
            return false;
        }

        if (Upto.CompareTo(other.From) <= 0 || other.Upto.CompareTo(From) <= 0)
        {
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{From}-{Upto}";
    }
}