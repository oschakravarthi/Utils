using SubhadraSolutions.Utils.Contracts;
using System;

namespace SubhadraSolutions.Utils.Abstractions;

[Serializable]
public abstract class AbstractInitializableAndDisposable : AbstractDisposable, IInitializable
{
    private readonly object syncLock = new();

    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        if (!IsInitialized)
        {
            lock (syncLock)
            {
                if (!IsInitialized)
                {
                    try
                    {
                        InitializeProtected();
                    }
                    finally
                    {
                        IsInitialized = true;
                    }
                }
            }
        }
    }

    protected void CheckAndThrowObjectAlreadyInitializedException()
    {
        if (IsInitialized)
        {
            throw new ObjectAlreadyInitializedException(AbstractInitializable
                .ObjectAlreadyInitializedExceptionMessage);
        }
    }

    protected abstract void InitializeProtected();
}