using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public class SafeListDecorator<T>(IList<T> actual) : SafeCollectionDecorator<T>(actual), IList<T>
{
    //Guard.ArgumentShouldNotBeNull(actual, nameof(actual));

    public T this[int index]
    {
        get
        {
            try
            {
                LockSlim.EnterReadLock();
                return actual[index];
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }

        set
        {
            try
            {
                LockSlim.EnterWriteLock();
                actual[index] = value;
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }
    }

    public int IndexOf(T item)
    {
        try
        {
            LockSlim.EnterReadLock();
            return actual.IndexOf(item);
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }

    public void Insert(int index, T item)
    {
        try
        {
            LockSlim.EnterWriteLock();
            actual.Insert(index, item);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public void RemoveAt(int index)
    {
        try
        {
            LockSlim.EnterWriteLock();
            actual.RemoveAt(index);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }
}