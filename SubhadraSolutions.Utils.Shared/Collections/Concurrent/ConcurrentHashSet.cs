using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

[DebuggerDisplay("Count = {Count}")]
public class ConcurrentHashSet<T> : AbstractDisposable, ICollection<T>, ISet<T>, IReadOnlyCollection<T>, IReadOnlySet<T>, IDeserializationCallback, IAtomicOperationSupported
{
    private readonly HashSet<T> _hashSet;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public ConcurrentHashSet()
    {
        _hashSet = [];
    }

    public ConcurrentHashSet(IEqualityComparer<T> comparer)
    {
        _hashSet = new HashSet<T>(comparer);
    }

    public ConcurrentHashSet(IEnumerable<T> collection)
    {
        _hashSet = new HashSet<T>(collection);
    }

    public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
    {
        _hashSet = new HashSet<T>(collection, comparer);
    }

    public int Count
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _hashSet.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    public bool IsReadOnly
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return ((ICollection<T>)_hashSet).IsReadOnly;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    public bool Add(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _hashSet.Add(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    void ICollection<T>.Add(T item)
    {
        this.Add(item);
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _hashSet.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.Contains(item);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _lock.EnterReadLock();
        try
        {
            _hashSet.CopyTo(array, arrayIndex);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        _lock.EnterWriteLock();
        try
        {
            _hashSet.ExceptWith(other);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new SafeEnumeratorDecorator<T>(_hashSet.GetEnumerator(), this._lock);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        _lock.EnterWriteLock();
        try
        {
            _hashSet.IntersectWith(other);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.IsProperSubsetOf(other);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.IsProperSupersetOf(other);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.IsSubsetOf(other);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.IsSupersetOf(other);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void OnDeserialization(object sender)
    {
        _hashSet.OnDeserialization(sender);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.Overlaps(other);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void PerformAtomicOperation(Action action)
    {
        _lock.EnterWriteLock();
        try
        {
            action();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool Remove(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _hashSet.Remove(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        _lock.EnterWriteLock();
        try
        {
            return _hashSet.SetEquals(other);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        _lock.EnterWriteLock();
        try
        {
            _hashSet.SymmetricExceptWith(other);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        _lock.EnterWriteLock();
        try
        {
            _hashSet.UnionWith(other);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    bool ISet<T>.Add(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _hashSet.Add(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _lock?.Dispose();
        }
    }
}