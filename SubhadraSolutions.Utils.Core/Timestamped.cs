using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class Timestamped<T> : AbstractTimestamped
{
    public Timestamped(DateTime timestamp, T info) : base(timestamp)
    {
        Info = info;
    }

    [JsonInclude] public T Info { get; private set; }

    public override string ToString()
    {
        return $"{base.ToString()} - {Info}";
    }
}