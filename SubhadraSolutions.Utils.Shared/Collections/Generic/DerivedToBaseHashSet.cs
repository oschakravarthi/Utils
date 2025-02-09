using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class DerivedToBaseHashSet<TDerived, TBase> : HashSet<TDerived>, ICollection<TBase>
    where TDerived : class, TBase
{
    int ICollection<TBase>.Count => Count;

    bool ICollection<TBase>.IsReadOnly => false;

    void ICollection<TBase>.Add(TBase item)
    {
        var derived = (TDerived)item;
        Add(derived);
    }

    void ICollection<TBase>.Clear()
    {
        Clear();
    }

    bool ICollection<TBase>.Contains(TBase item)
    {
        var derived = (TDerived)item;
        return Contains(derived);
    }

    void ICollection<TBase>.CopyTo(TBase[] array, int arrayIndex)
    {
        var index = 0;
        foreach (var v in this)
        {
            array[index + arrayIndex] = v;
            index++;
        }
    }

    IEnumerator<TBase> IEnumerable<TBase>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool ICollection<TBase>.Remove(TBase item)
    {
        var derived = (TDerived)item;
        return Remove(derived);
    }
}