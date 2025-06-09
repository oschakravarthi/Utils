using System;

namespace SubhadraSolutions.Utils;

public class LocalableDateTime
{
    public LocalableDateTime(DateTime value)
    {
        Value = value;
    }

    public DateTime Value { get; }
}