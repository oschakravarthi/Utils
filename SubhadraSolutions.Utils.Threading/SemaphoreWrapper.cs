using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Threading;

public sealed class SemaphoreWrapper(int initialCount, int maximumCount) : AbstractDisposable
{
    private readonly Semaphore _semaphore = new(initialCount, maximumCount);

    ~SemaphoreWrapper()
    {
        Dispose(false);
    }

    public long QueueupItems(IEnumerable enumerable, WaitCallback callback, bool returnOnlyAfterCompletingAll,
        bool suppressContext = false)
    {
        CheckAndThrowDisposedException();
        var isContextSupressed = false;
        try
        {
            if (suppressContext)
            {
                try
                {
                    ExecutionContext.SuppressFlow();
                    isContextSupressed = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToDetailedString());
                }
            }

            var pendingCount = new PendingCount();
            long count = 0;
            foreach (var obj in enumerable)
            {
                while (true)
                {
                    if (IsDisposed)
                    {
                        return count;
                    }

                    if (_semaphore.WaitOne(500))
                    {
                        count++;

                        Task.Factory.StartNew(state =>
                        {
                            try
                            {
                                pendingCount.Increment();
                                callback(state);
                            }
                            finally
                            {
                                _semaphore.Release();
                                pendingCount.Decrement();
                            }
                        }, obj);
                        break;
                    }

                    Thread.Yield();
                }
            }

            if (returnOnlyAfterCompletingAll)
            {
                while (pendingCount.Count != 0)
                    Thread.Yield();
            }

            return count;
        }
        catch (ObjectDisposedException)
        {
            return 0;
        }
        finally
        {
            if (suppressContext && isContextSupressed)
            {
                try
                {
                    ExecutionContext.RestoreFlow();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        _semaphore.Dispose();
    }

    public class PendingCount
    {
        private long _pendingCount;

        public long Count => Interlocked.Read(ref _pendingCount);

        public void Decrement()
        {
            Interlocked.Decrement(ref _pendingCount);
        }

        public void Increment()
        {
            Interlocked.Increment(ref _pendingCount);
        }
    }
}