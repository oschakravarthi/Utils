using SubhadraSolutions.Utils.Validation;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class SafeEnumerableDecorator<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> adaptedObject;

    private readonly ReaderWriterLock readerWriterLock;

    private readonly ReaderWriterLockSlim readerWriterLockSlim;

    public SafeEnumerableDecorator(IEnumerable<T> adaptedObject, ReaderWriterLock readerWriterLock)
        : this(adaptedObject)
    {
        Guard.ArgumentShouldNotBeNull(readerWriterLock, nameof(readerWriterLock));
        this.readerWriterLock = readerWriterLock;
    }

    public SafeEnumerableDecorator(IEnumerable<T> adaptedObject, ReaderWriterLockSlim readerWriterLockSlim)
        : this(adaptedObject)
    {
        Guard.ArgumentShouldNotBeNull(readerWriterLockSlim, nameof(readerWriterLockSlim));
        this.readerWriterLockSlim = readerWriterLockSlim;
    }

    private SafeEnumerableDecorator(IEnumerable<T> adaptedObject)
    {
        Guard.ArgumentShouldNotBeNull(adaptedObject, nameof(adaptedObject));
        this.adaptedObject = adaptedObject;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (readerWriterLock != null)
        {
            return new SafeEnumeratorDecorator<T>(adaptedObject.GetEnumerator(), readerWriterLock);
        }

        return new SafeEnumeratorDecorator<T>(adaptedObject.GetEnumerator(), readerWriterLockSlim);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}