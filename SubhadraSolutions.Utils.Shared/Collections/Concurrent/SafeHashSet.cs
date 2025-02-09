using SubhadraSolutions.Utils.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class SafeHashSet<T>() : AbstractReaderWriterAtomicOperationSupported(true), ICollection<T>
{
    private readonly HashSet<T> adaptedObject = [];

    public int Count => adaptedObject.Count;

    public bool IsReadOnly => false;

    public void Clear()
    {
        try
        {
            LockSlim.EnterWriteLock();
            adaptedObject.Clear();
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        LockSlim.EnterReadLock();
        try
        {
            return adaptedObject.Contains(item);
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
            adaptedObject.CopyTo(array, arrayIndex);
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new SafeEnumeratorDecorator<T>(adaptedObject.GetEnumerator(), LockSlim);
    }

    public bool Remove(T item)
    {
        try
        {
            LockSlim.EnterWriteLock();
            return adaptedObject.Remove(item);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Add(T item)
    {
        try
        {
            LockSlim.EnterWriteLock();
            return adaptedObject.Add(item);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }

    public List<T> GetAll()
    {
        LockSlim.EnterReadLock();
        try
        {
            return adaptedObject.ToList();
        }
        finally
        {
            LockSlim.ExitReadLock();
        }
    }
}