using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class SafeLinkedList<T>(LinkedList<T> adaptedObject) : SafeCollectionDecorator<T>(adaptedObject)
{
    public T RemoveFirst()
    {
        LockSlim.EnterUpgradeableReadLock();
        try
        {
            var firstNode = adaptedObject.First;
            if (firstNode == null)
            {
                adaptedObject.RemoveFirst();
                return default;
            }

            LockSlim.EnterWriteLock();
            try
            {
                adaptedObject.RemoveFirst();
                return firstNode.Value;
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }
        finally
        {
            LockSlim.ExitUpgradeableReadLock();
        }
    }

    public bool TryRemoveFirst(out T item)
    {
        item = default;
        LockSlim.EnterUpgradeableReadLock();
        try
        {
            var firstNode = adaptedObject.First;
            if (firstNode == null)
            {
                return false;
            }

            LockSlim.EnterWriteLock();
            try
            {
                adaptedObject.RemoveFirst();
                item = firstNode.Value;
                return true;
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }
        finally
        {
            LockSlim.ExitUpgradeableReadLock();
        }
    }
}