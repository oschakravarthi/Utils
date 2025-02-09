using SubhadraSolutions.Utils.Contracts;
using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Abstractions;

public abstract class AbstractTimestamped : ITimestamped
{
    protected AbstractTimestamped(DateTime timestamp)
    {
        Timestamp = timestamp;
    }

    [JsonInclude] public DateTime Timestamp { get; }

    public override string ToString()
    {
        return $"{Timestamp}";
    }
}