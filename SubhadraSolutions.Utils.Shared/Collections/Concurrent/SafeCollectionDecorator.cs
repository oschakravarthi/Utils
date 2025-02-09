using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.Threading;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public class SafeCollectionDecorator<T>(ICollection<T> actual) : AbstractReaderWriterAtomicOperationSupported(true),
    ICollection<T>, IClearable
{
    //Guard.ArgumentShouldNotBeNull(actual, nameof(actual));

    public int Count
    {
        get
        {
            try
            {
                LockSlim.EnterReadLock();
                return actual.Count;
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }
    }

    public bool IsReadOnly
    {
        get
        {
            try
            {
                LockSlim.EnterReadLock();
                return actual.IsReadOnly;
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }
    }

    public void Add(T item)
    {
        try
        {
            LockSlim.EnterWriteLock();
            actual.Add(item);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public void Clear()
    {
        try
        {
            LockSlim.EnterWriteLock();
            actual.Clear();
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        try
        {
            LockSlim.EnterReadLock();
            return actual.Contains(item);
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        try
        {
            LockSlim.EnterReadLock();
            actual.CopyTo(array, arrayIndex);
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    public bool Remove(T item)
    {
        try
        {
            LockSlim.EnterWriteLock();
            return actual.Remove(item);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    private IEnumerator<T> GetEnumeratorCore()
    {
        var enumerator = actual.GetEnumerator();
        enumerator = new SafeEnumeratorDecorator<T>(enumerator, LockSlim);
        return enumerator;
    }
}