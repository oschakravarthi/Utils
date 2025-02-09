using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class DateTimeRangeComparer<T> : IComparer<T> where T : Range<DateTime>
{
    public static readonly DateTimeRangeComparer<T> Instance = new();

    private DateTimeRangeComparer()
    {
    }

    public int Compare(T x, T y)
    {
        return x.From.CompareTo(y.From);
    }
}