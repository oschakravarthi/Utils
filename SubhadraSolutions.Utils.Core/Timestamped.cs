using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Contracts;
using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class Timestamped<T> : AbstractTimestamped, IToLocal<Timestamped<T>>
{
    public Timestamped(DateTime timestamp, T info) : base(timestamp)
    {
        Info = info;
    }

    [JsonInclude] public T Info { get; private set; }

    public Timestamped<T> ToLocal(TimeZoneInfo timezone)
    {
        var info = Info;
        if (info is IToLocal<T> infoAsToLocal)
        {
            info = infoAsToLocal.ToLocal(timezone);
        }

        var convertedTimestamp = TimeZoneInfo.ConvertTimeFromUtc(Timestamp, timezone);
        convertedTimestamp = new DateTime(convertedTimestamp.Ticks, DateTimeKind.Local);
        return new Timestamped<T>(convertedTimestamp, info);
    }

    public override string ToString()
    {
        return $"{base.ToString()} - {Info}";
    }
}