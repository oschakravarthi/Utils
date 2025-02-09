using System;

namespace SubhadraSolutions.Utils.Pooling;

public class GenericPool<T> : AbstractPool<T, GenericPoolItem<T>>
    where T : new()
{
    private readonly IAdaptedObjectSelectionStrategy<T> _strategy;

    public GenericPool(IAdaptedObjectSelectionStrategy<T> strategy = null)
    {
        _strategy = strategy;
    }

    public GenericPool(int maxPoolSize, int objectMaxIdleTimeInMilliseconds, bool canObjectBeShared,
        IAdaptedObjectSelectionStrategy<T> strategy = null)
        : base(maxPoolSize, objectMaxIdleTimeInMilliseconds, canObjectBeShared)
    {
        _strategy = strategy;
    }

    public IAsyncResult BeginGetObject()
    {
        return BeginGetObject(null);
    }

    public IAsyncResult BeginTryGetObject(int millisecondsToWait)
    {
        return BeginTryGetObject(null, millisecondsToWait);
    }

    public GenericPoolItem<T> EndGetObject(IAsyncResult result)
    {
        return EndGetObjectCore(result);
    }

    public bool EndTryGetObject(IAsyncResult result, out GenericPoolItem<T> obj)
    {
        return EndTryGetObjectCore(result, out obj);
    }

    public GenericPoolItem<T> GetObject()
    {
        return GetObject(null);
    }

    public bool TryGetObject(out GenericPoolItem<T> obj, int millisecondsToWait)
    {
        return TryGetObject(out obj, null, millisecondsToWait);
    }

    protected override bool CanSelectThisAdaptedObject(T adaptedObject, bool isNewlyCreatedAdaptedObject, object tag)
    {
        if (_strategy != null)
        {
            return true;
        }

        return base.CanSelectThisAdaptedObject(adaptedObject, isNewlyCreatedAdaptedObject, tag);
    }

    protected override T GetAdaptedObject()
    {
        return new T();
    }
}