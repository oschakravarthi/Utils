using SubhadraSolutions.Utils.Contracts;
using System;

namespace SubhadraSolutions.Utils;

public class LocalableDateTime : IToLocal<LocalableDateTime>
{
    public LocalableDateTime(DateTime value)
    {
        Value = value;
    }

    public DateTime Value { get; }

    public LocalableDateTime ToLocal(TimeZoneInfo timezone)
    {
        return new LocalableDateTime(TimeZoneInfo.ConvertTimeFromUtc(Value, timezone));
    }
}