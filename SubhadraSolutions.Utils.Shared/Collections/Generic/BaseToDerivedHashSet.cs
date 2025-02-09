using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class BaseToDerivedHashSet<TBase, TDerived> : HashSet<TBase>, ICollection<TDerived>
    where TDerived : class, TBase
{
    int ICollection<TDerived>.Count => Count;

    bool ICollection<TDerived>.IsReadOnly => false;

    void ICollection<TDerived>.Add(TDerived item)
    {
        Add(item);
    }

    void ICollection<TDerived>.Clear()
    {
        Clear();
    }

    bool ICollection<TDerived>.Contains(TDerived item)
    {
        return Contains(item);
    }

    void ICollection<TDerived>.CopyTo(TDerived[] array, int arrayIndex)
    {
        CopyTo(array, arrayIndex);
    }

    IEnumerator<TDerived> IEnumerable<TDerived>.GetEnumerator()
    {
        foreach (var item in this)
        {
            yield return (TDerived)item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool ICollection<TDerived>.Remove(TDerived item)
    {
        return Remove(item);
    }
}