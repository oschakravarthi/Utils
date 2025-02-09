using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public class GenericFlyweight<T>
    (Func<object, T> createObjectFunc, bool useWeakReferences) : AbstractMonitorAtomicOperationSupported
    where T : class
{
    private readonly Dictionary<object, object> dictionary = [];

    //Guard.ArgumentShouldNotBeNull(createObjectFunc, nameof(createObjectFunc));

    public bool UsesWeakReferences { get; } = useWeakReferences;

    public void Clear()
    {
        lock (syncLockObject)
        {
            dictionary.Clear();
        }
    }

    public T GetObject(object flyweightSpecification)
    {
        //Guard.ArgumentShouldNotBeNull(flyweightSpecification, nameof(flyweightSpecification));
        lock (syncLockObject)
        {
            if (!dictionary.TryGetValue(flyweightSpecification, out var obj))
            {
                return CreateObjectAndAdd(flyweightSpecification);
            }

            if (TryCastObject(obj, out var castedObject))
            {
                return castedObject;
            }

            dictionary.Remove(flyweightSpecification);
            return CreateObjectAndAdd(flyweightSpecification);
        }
    }

    public bool Remove(object flyweightSpecification)
    {
        lock (syncLockObject)
        {
            return dictionary.Remove(flyweightSpecification);
        }
    }

    private T CreateObjectAndAdd(object flyweightSpecification)
    {
        var item = createObjectFunc(flyweightSpecification);
        object obj = item;
        if (item != null)
        {
            if (UsesWeakReferences)
            {
                obj = new WeakReference(obj);
            }
        }

        dictionary.Add(flyweightSpecification, obj);
        return item;
    }

    private bool TryCastObject(object obj, out T castedObject)
    {
        castedObject = null;
        if (obj == null)
        {
            return true;
        }

        if (!UsesWeakReferences)
        {
            castedObject = (T)obj;
            return true;
        }

        var weakReference = (WeakReference)obj;
        var target = weakReference.Target;
        if (target != null)
        {
            castedObject = (T)target;
            return true;
        }

        return false;
    }
}