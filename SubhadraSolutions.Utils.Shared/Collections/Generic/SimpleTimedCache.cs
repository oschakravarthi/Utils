using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Generic;

public delegate T GetCacheObjectCallback<out T>();

public sealed class SimpleTimedCache<T>(long interval, GetCacheObjectCallback<T> callback) : AbstractDisposable
{
    private readonly ReaderWriterLockSlim readerWriterLockSlim = new();

    private T cacheObject;

    private DateTime? lastCreationTime;

    public long Interval { get; } = interval;

    public T GetObject()
    {
        readerWriterLockSlim.EnterUpgradeableReadLock();
        try
        {
            if (ShouldRefreshCache())
            {
                readerWriterLockSlim.EnterWriteLock();
                try
                {
                    if (ShouldRefreshCache())
                    {
                        refreshCache();
                    }
                }
                finally
                {
                    readerWriterLockSlim.ExitWriteLock();
                }
            }
        }
        finally
        {
            readerWriterLockSlim.ExitUpgradeableReadLock();
        }

        return cacheObject;
    }

    protected override void Dispose(bool disposing)
    {
        readerWriterLockSlim.Dispose();
    }

    private void refreshCache()
    {
        cacheObject = callback();
        lastCreationTime = GeneralHelper.CurrentDateTime;
    }

    private bool ShouldRefreshCache()
    {
        if (lastCreationTime == null)
        {
            return true;
        }

        if ((GeneralHelper.CurrentDateTime - lastCreationTime.Value).TotalMilliseconds >= Interval)
        {
            return true;
        }

        return false;
    }
}