using System;

namespace SubhadraSolutions.Utils;

public sealed class GenericEventArgs<T> : EventArgs
{
    public GenericEventArgs(T payload)
    {
        Payload = payload;
    }

    public T Payload { get; private set; }
}