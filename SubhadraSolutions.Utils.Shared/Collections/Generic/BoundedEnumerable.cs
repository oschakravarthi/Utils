using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class BoundedEnumerable<T>(IDataStructureAdapter<T> store, int startIndex, int endIndex)
    : IEnumerable<T>
    where T : class
{
    public virtual IEnumerator<T> GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    protected virtual IEnumerator<T> GetEnumeratorCore()
    {
        return new DataStructureEnumerator<T>(store, startIndex, endIndex);
    }
}