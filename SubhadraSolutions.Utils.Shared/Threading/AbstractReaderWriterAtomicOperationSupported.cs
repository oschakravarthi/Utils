using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Threading;

namespace SubhadraSolutions.Utils.Threading;

public abstract class AbstractReaderWriterAtomicOperationSupported : AbstractDisposable, IAtomicOperationSupported
{
    protected AbstractReaderWriterAtomicOperationSupported(bool shouldBeThreadSafe)
    {
        if (shouldBeThreadSafe)
        {
            LockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }
    }

    protected ReaderWriterLockSlim LockSlim { get; }

    public void PerformAtomicOperation(Action action)
    {
        //Guard.ArgumentShouldNotBeNull(action, nameof(action));
        try
        {
            LockSlim?.EnterWriteLock();

            action();
        }
        finally
        {
            LockSlim?.ExitWriteLock();
        }
    }

    protected override void Dispose(bool disposing)
    {
        LockSlim?.Dispose();
    }
}