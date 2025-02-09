using System;

namespace SubhadraSolutions.Utils.Pooling;

public interface IPoolItem<T> : IDisposable
{
    void SetAdaptedObjectAndCallback(T adaptedObject, ReturnAdaptedObjectToPoolDelegate<T> callback);
}