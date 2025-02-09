using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class InverseComparer<T> : IComparer<T>
{
    private readonly IComparer<T> inner;

    public InverseComparer(IComparer<T> inner)
    {
        this.inner = inner;
    }

    public int Compare(T x, T y)
    {
        return -inner.Compare(x, y);
    }
}