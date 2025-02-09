using System;

namespace SubhadraSolutions.Utils;

[Serializable]
public sealed class ObjectAlreadyInitializedException : InvalidOperationException
{
    public ObjectAlreadyInitializedException()
    {
    }

    public ObjectAlreadyInitializedException(string message)
        : base(message)
    {
    }
}