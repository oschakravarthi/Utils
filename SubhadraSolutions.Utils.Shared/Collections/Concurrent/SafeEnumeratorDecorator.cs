using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class SafeEnumeratorDecorator<T> : AbstractDisposable, IEnumerator<T>
{
    private readonly IEnumerator<T> actual;

    private readonly ReaderWriterLock readerWriterLock;

    private readonly ReaderWriterLockSlim readerWriterLockSlim;

    private volatile bool hasLockAcquired;

    public SafeEnumeratorDecorator(IEnumerator<T> actual, ReaderWriterLock readerWriterLock)
        : this(actual)
    {
        //Guard.ArgumentShouldNotBeNull(readerWriterLock, nameof(readerWriterLock));
        this.readerWriterLock = readerWriterLock;
    }

    public SafeEnumeratorDecorator(IEnumerator<T> actual, ReaderWriterLockSlim readerWriterLockSlim)
        : this(actual)
    {
        //Guard.ArgumentShouldNotBeNull(readerWriterLockSlim, nameof(readerWriterLockSlim));
        this.readerWriterLockSlim = readerWriterLockSlim;
    }

    private SafeEnumeratorDecorator(IEnumerator<T> actual)
    {
        //Guard.ArgumentShouldNotBeNull(actual, nameof(actual));
        this.actual = actual;
    }

    public T Current
    {
        get
        {
            CheckAndThrowDisposedException();
            return actual.Current;
        }
    }

    object IEnumerator.Current => actual.Current;

    public bool MoveNext()
    {
        CheckAndThrowDisposedException();
        AcquireLock();
        var canMoveNext = actual.MoveNext();
        if (!canMoveNext)
        {
            ReleaseLock();
        }

        return canMoveNext;
    }

    public void Reset()
    {
        CheckAndThrowDisposedException();
        actual.Reset();
        ReleaseLock();
    }

    protected override void Dispose(bool disposing)
    {
        ReleaseLock();
    }

    private void AcquireLock()
    {
        if (!hasLockAcquired)
        {
            if (readerWriterLock != null)
            {
                readerWriterLock.AcquireReaderLock(Timeout.Infinite);
            }
            else
            {
                readerWriterLockSlim.EnterReadLock();
            }

            hasLockAcquired = true;
        }
    }

    private void ReleaseLock()
    {
        if (hasLockAcquired)
        {
            if (readerWriterLock != null)
            {
                readerWriterLock.ReleaseReaderLock();
            }
            else
            {
                readerWriterLockSlim.ExitReadLock();
            }

            hasLockAcquired = false;
        }
    }
}