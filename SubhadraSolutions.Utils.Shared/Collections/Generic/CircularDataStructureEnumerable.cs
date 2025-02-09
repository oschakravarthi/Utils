using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class CircularDataStructureEnumerable<T>(IDataStructureAdapter<T> store, int startIndex, int lastWrittenIndex,
        bool isCycleCompleted)
    : IEnumerable<T>
    where T : class
{
    private readonly bool isCycleCompleted = isCycleCompleted;

    private readonly int lastWrittenIndex = lastWrittenIndex;

    private readonly int startIndex = startIndex;

    private readonly IDataStructureAdapter<T> store = store;

    public IEnumerator<T> GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    protected virtual IEnumerator<T> GetEnumeratorCore()
    {
        return new CircularDataStructureEnumerator<T>(store, startIndex, lastWrittenIndex, isCycleCompleted);
    }
}