using System;

namespace SubhadraSolutions.Utils.Caching;

public struct ValueAndExpiry<T>(T value, TimeSpan? expiry = null)
{
    public TimeSpan? Expiry { get; private set; } = expiry;
    public T Value { get; private set; } = value;
}