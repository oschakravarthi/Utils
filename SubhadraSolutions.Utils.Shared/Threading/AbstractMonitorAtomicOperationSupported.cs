using System;

namespace SubhadraSolutions.Utils.Threading;

public abstract class AbstractMonitorAtomicOperationSupported : IAtomicOperationSupported
{
    protected readonly object syncLockObject = new();

    public void PerformAtomicOperation(Action action)
    {
        lock (syncLockObject)
        {
            action();
        }
    }
}