using System;

namespace SubhadraSolutions.Utils.Abstractions;

[Serializable]
public abstract class AbstractDisposable : IDisposable
{
    private const string ObjectDisposedExceptionMessage = "Object already disposed.";

    private readonly object syncLock = new();

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            lock (syncLock)
            {
                if (!IsDisposed)
                {
                    try
                    {
                        Dispose(true);
                    }
                    finally
                    {
                        IsDisposed = true;
                        GC.SuppressFinalize(this);
                    }
                }
            }
        }
    }

    protected void CheckAndThrowDisposedException()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(ObjectDisposedExceptionMessage);
        }
    }

    protected abstract void Dispose(bool disposing);
}