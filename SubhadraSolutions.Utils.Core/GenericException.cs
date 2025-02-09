using System;

namespace SubhadraSolutions.Utils;

public class GenericException<T> : Exception
{
    public GenericException(T payload)
    {
        this.Payload = payload;
    }

    public GenericException(T payload, string message) : base(message)
    {
        this.Payload = payload;
    }

    public GenericException(T payload, string message, Exception innerException) : base(message, innerException)
    {
        this.Payload = payload;
    }

    public T Payload { get; private set; }
}