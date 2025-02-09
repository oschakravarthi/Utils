using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public sealed class CloneEnumerable<T>(IEnumerable<T> adaptedObject) : IEnumerable<T>
    where T : ICloneable
{
    private readonly IEnumerable<T> adaptedObject = adaptedObject;

    public IEnumerator<T> GetEnumerator()
    {
        return new CloneEnumerator<T>(adaptedObject.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}