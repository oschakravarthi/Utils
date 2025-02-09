using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class CloneEnumerator<T>(IEnumerator<T> adaptedObject) : AbstractDisposable, IEnumerator<T>
    where T : ICloneable
{
    object IEnumerator.Current => GetCurrent();

    T IEnumerator<T>.Current => GetCurrent();

    public bool MoveNext()
    {
        return adaptedObject.MoveNext();
    }

    public void Reset()
    {
        adaptedObject.Reset();
    }

    protected override void Dispose(bool disposing)
    {
        adaptedObject.Dispose();
    }

    private T GetCurrent()
    {
        var adaptedObjectCurrent = adaptedObject.Current;
        if (adaptedObjectCurrent is IDeepCloneable deepCloneable)
        {
            return (T)deepCloneable.DeepClone();
        }

        if (adaptedObjectCurrent is ICloneable cloneable)
        {
            return (T)cloneable.Clone();
        }

        return (T)adaptedObjectCurrent.Clone();
    }
}