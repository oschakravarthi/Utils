using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Threading;

namespace SubhadraSolutions.Utils.Threading;

public class AsyncResult<T> : AbstractDisposable, IAsyncResult
{
    private readonly ManualResetEvent _waitHandle = new(false);

    public Exception Exception { get; private set; }

    public T TypedAsyncState { get; internal set; }

    public object AsyncState => TypedAsyncState;

    public WaitHandle AsyncWaitHandle => _waitHandle;

    public bool CompletedSynchronously => false;

    public bool IsCompleted { get; private set; }

    public void SetCompleted(T returnedValue)
    {
        IsCompleted = true;
        TypedAsyncState = returnedValue;
        _waitHandle.Set();
    }

    internal void SetCompletedWithException(Exception ex)
    {
        IsCompleted = true;
        Exception = ex;
        _waitHandle.Set();
    }

    protected override void Dispose(bool disposing)
    {
        AsyncWaitHandle.Dispose();
    }
}