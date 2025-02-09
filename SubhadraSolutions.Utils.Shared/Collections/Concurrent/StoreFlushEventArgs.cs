using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class StoreFlushEventArgs<T>(T[] storeData, int fromIndex, int toIndex) : EventArgs, IEnumerable<T>
{
    public StoreFlushEventArgs(T[] storeData)
        : this(storeData, 0, storeData.Length - 1)
    {
    }

    public int Length => ToIndex - FromIndex + 1;

    internal int FromIndex { get; } = fromIndex;

    internal T[] StoreData { get; } = storeData;

    internal int ToIndex { get; } = toIndex;

    public T this[int index] => StoreData[index + FromIndex];

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = FromIndex; i <= ToIndex; i++) yield return StoreData[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Sort(IComparer<T> comparer)
    {
        Array.Sort(StoreData, FromIndex, ToIndex - FromIndex + 1, comparer);
    }
}