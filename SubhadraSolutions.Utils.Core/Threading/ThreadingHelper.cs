using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Threading;

public delegate T AsyncDelegate<out T>();

public static class ThreadingHelper
{
    private static readonly ReaderWriterLockSlim internalLock = new(LockRecursionPolicy.SupportsRecursion);

    private static readonly ConcurrentDictionary<object, int> lockReferences = new();

    private static readonly ConcurrentDictionary<object, ReaderWriterLockSlim> lockTable = new();

    public static AsyncResult<T> BeginExecute<T>(AsyncDelegate<T> asyncDelegate, AsyncCallback callback)
    {
        var result = new AsyncResult<T>();
        Task.Factory.StartNew(delegate
        {
            try
            {
                var returnedValue = asyncDelegate();
                result.SetCompleted(returnedValue);
            }
            catch (Exception ex)
            {
                result.SetCompletedWithException(ex);
            }

            if (callback != null)
            {
                callback(result);
            }
        });
        return result;
    }

    public static T EndExecute<T>(AsyncResult<T> result)
    {
        result.AsyncWaitHandle.WaitOne();
        if (result.Exception != null)
        {
            throw result.Exception;
        }

        return result.TypedAsyncState;
    }

    public static void ExchangeIfGreaterThan(ref long location, long value)
    {
        exchangeIfGreaterThanOrLessThan(ref location, value, true);
    }

    public static void ExchangeIfLessThan(ref long location, long value)
    {
        exchangeIfGreaterThanOrLessThan(ref location, value, false);
    }

    public static void ExecuteParallel(IEnumerable<Action> actions, bool waitForAllToFinish)
    {
        if (!waitForAllToFinish)
        {
            foreach (var action in actions)
            {
                Task.Factory.StartNew(state =>
                {
                    var act = (Action)state;
                    act();
                }, action);
            }
        }
        else
        {
            var countDownEvent = new CountdownEvent(0);
            foreach (var action in actions)
            {
                countDownEvent.AddCount();
                Task.Factory.StartNew(state =>
                {
                    var act = (Action)state;
                    try
                    {
                        act();
                    }
                    finally
                    {
                        countDownEvent.Signal();
                    }
                }, action);
            }

            countDownEvent.Wait();
            countDownEvent.Dispose();
        }
    }

    public static object GetDataFromThreadLocalStorage(string slotName)
    {
        //Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(slotName, nameof(slotName));
        var slot = Thread.GetNamedDataSlot(slotName);
        return Thread.GetData(slot);
    }

    public static void RemoveDataFromThreadLocalStorage(string slotName)
    {
        //Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(slotName, nameof(slotName));
        Thread.FreeNamedDataSlot(slotName);
    }

    public static TResult SafeDeref<TResult>(Func<TResult> action)
    {
        TResult result = default;
        try
        {
            if (action != null)
            {
                result = action();
                return result;
            }

            return result;
        }
        catch (NullReferenceException)
        {
            return result;
        }
        catch (ArgumentNullException)
        {
            return result;
        }
    }

    public static void StoreDataInThreadLocalStorage(string slotName, object data)
    {
        //Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(slotName, nameof(slotName));
        var slot = Thread.GetNamedDataSlot(slotName);
        Thread.SetData(slot, data);
    }

    public static TResult ThreadSafeRead<TObject, TResult>(this TObject pThis, Func<TObject, TResult> operation)
    {
        var readerWriteLock = GetReaderWriteLock(pThis);
        readerWriteLock.EnterReadLock();
        try
        {
            return operation(pThis);
        }
        finally
        {
            readerWriteLock.ExitReadLock();
            ReleaseReaderWriteLock(pThis);
        }
    }

    public static void ThreadSafeRead<TObject>(this TObject pThis, Action<TObject> operation)
    {
        var readerWriteLock = GetReaderWriteLock(pThis);
        readerWriteLock.EnterReadLock();
        try
        {
            operation(pThis);
        }
        finally
        {
            readerWriteLock.ExitReadLock();
            ReleaseReaderWriteLock(pThis);
        }
    }

    public static TResult ThreadSafeWrite<TObject, TResult>(this TObject pThis, Func<TObject, TResult> operation)
    {
        var readerWriteLock = GetReaderWriteLock(pThis);
        readerWriteLock.EnterWriteLock();
        try
        {
            return operation(pThis);
        }
        finally
        {
            readerWriteLock.ExitWriteLock();
            ReleaseReaderWriteLock(pThis);
        }
    }

    public static void ThreadSafeWrite<TObject>(this TObject pThis, Action<TObject> operation)
    {
        var readerWriteLock = GetReaderWriteLock(pThis);
        readerWriteLock.EnterWriteLock();
        try
        {
            operation(pThis);
        }
        finally
        {
            readerWriteLock.ExitWriteLock();
            ReleaseReaderWriteLock(pThis);
        }
    }

    public static Task<T> WrapTask<T>(Task<object> task)
    {
        return task.ContinueWith(parentTask => (T)parentTask.Result, TaskContinuationOptions.ExecuteSynchronously);
    }

    private static void exchangeIfGreaterThanOrLessThan(ref long location, long value, bool greaterThan)
    {
        var current = Interlocked.Read(ref location);
        while ((greaterThan && current < value) || (!greaterThan && current > value))
        {
            var previous = Interlocked.CompareExchange(ref location, value, current);
            if (previous == current || (greaterThan && previous >= value) ||
                (!greaterThan && previous <= value))
            {
                break;
            }

            current = Interlocked.Read(ref location);
        }
    }

    private static ReaderWriterLockSlim GetReaderWriteLock(object item)
    {
        if (lockTable.TryGetValue(item, out var value))
        {
            return value;
        }

        internalLock.EnterWriteLock();
        try
        {
            lockReferences.AddOrUpdate(item, 1, (o, i) => i < 0 ? 1 : i++);
            return lockTable.GetOrAdd(item, new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion));
        }
        finally
        {
            internalLock.ExitWriteLock();
        }
    }

    private static void ReleaseReaderWriteLock(object item)
    {
        internalLock.EnterWriteLock();
        try
        {
            if (lockReferences.AddOrUpdate(item, 0, (o, i) => i >= 1 ? i-- : 0) < 1)
            {
                if (lockTable.TryRemove(item, out var value))
                {
                    value.Dispose();
                }
            }
        }
        finally
        {
            internalLock.ExitWriteLock();
        }
    }
}