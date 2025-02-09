using SubhadraSolutions.Utils.Contracts;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class TimestampedComparer<T> : IComparer<T> where T : ITimestamped
{
    public static readonly TimestampedComparer<T> Instance = new();

    private TimestampedComparer()
    {
    }

    public int Compare(T x, T y)
    {
        return x.Timestamp.CompareTo(y.Timestamp);
    }
}