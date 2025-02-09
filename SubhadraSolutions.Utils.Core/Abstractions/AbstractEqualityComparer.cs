using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Abstractions;

public abstract class AbstractEqualityComparer<T> : IEqualityComparer<T>
{
    public bool Equals(T x, T y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return EqualsProtected(x, y);
    }

    public virtual int GetHashCode(T obj)
    {
        return obj.GetHashCode();
    }

    protected abstract bool EqualsProtected(T x, T y);
}