using SubhadraSolutions.Utils.Contracts;
using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Abstractions;

public abstract class AbstractJulianTimestamped : IJulianTimestamped
{
    protected AbstractJulianTimestamped(JulianDateTime timestamp)
    {
        Timestamp = timestamp;
    }

    [JsonInclude] public JulianDateTime Timestamp { get; }

    public override string ToString()
    {
        return $"{Timestamp}";
    }
}