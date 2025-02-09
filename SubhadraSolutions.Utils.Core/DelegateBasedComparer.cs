using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class DelegateBasedComparer<T> : IComparer<T>
{
    private readonly Comparison<T> comparison;

    public DelegateBasedComparer(Comparison<T> comparison)
    {
        this.comparison = comparison;
    }

    public int Compare(T x, T y)
    {
        return comparison(x, y);
    }
}