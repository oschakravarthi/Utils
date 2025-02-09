using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class TimeOnlyComparer : IComparer<TimeOnly>
{
    private TimeOnlyComparer()
    {
    }

    public static TimeOnlyComparer Instance { get; } = new();

    public int Compare(TimeOnly x, TimeOnly y)
    {
        return x.CompareTo(y);
    }
}