using SubhadraSolutions.Utils.Abstractions;
using System;

namespace SubhadraSolutions.Utils.Pooling;

public class PoolItem<T> : AbstractDisposable, IPoolItem<T>
{
    private ReturnAdaptedObjectToPoolDelegate<T> _callback;
    private T adaptedObject;

    protected T AdaptedObjectProtected
    {
        get
        {
            if (!IsDisposed)
            {
                return adaptedObject;
            }

            throw new ObjectDisposedException("this",
                "The object is already disposed and the AdaptedObjectProtected is returned to the Pool");
        }
    }

    void IPoolItem<T>.SetAdaptedObjectAndCallback(T adaptedObject, ReturnAdaptedObjectToPoolDelegate<T> callback)
    {
        this.adaptedObject = adaptedObject;
        _callback = callback;
    }

    protected override void Dispose(bool disposing)
    {
        _callback(this, adaptedObject);
    }
}