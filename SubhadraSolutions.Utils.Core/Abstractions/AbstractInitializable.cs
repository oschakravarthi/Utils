using SubhadraSolutions.Utils.Contracts;
using System;
using System.Runtime.ConstrainedExecution;

namespace SubhadraSolutions.Utils.Abstractions;

[Serializable]
public abstract class AbstractInitializable : CriticalFinalizerObject, IInitializable
{
    public static readonly string ObjectAlreadyInitializedExceptionMessage =
        "Object already initialized. This operation can not performed after the abject is initialized.";

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
            throw new ObjectAlreadyInitializedException(ObjectAlreadyInitializedExceptionMessage);
        }
    }

    protected abstract void InitializeProtected();
}