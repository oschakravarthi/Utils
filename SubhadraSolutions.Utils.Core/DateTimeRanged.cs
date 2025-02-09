using SubhadraSolutions.Utils.Contracts;
using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class DateTimeRanged<I> : DateTimeRange, IToLocal<DateTimeRanged<I>>
{
    [Newtonsoft.Json.JsonConstructor]
    [JsonConstructor]
    public DateTimeRanged(DateTime from, DateTime upto, I info) : base(from, upto)
    {
        Info = info;
    }

    public DateTimeRanged(DateTime from, TimeSpan duration, I info) : base(from, from + duration)
    {
        Info = info;
    }

    public DateTimeRanged(Range<DateTime> range, I info) : base(range.From, range.Upto)
    {
        Info = info;
    }

    [JsonInclude]
    public I Info { get; private set; }

    public new DateTimeRanged<I> ToLocal(TimeZoneInfo timezone)
    {
        var info = Info;
        if (Info != null)
        {
            if (info is IToLocal<I> infoAsToLocal)
            {
                info = infoAsToLocal.ToLocal(timezone);
            }
        }

        return new DateTimeRanged<I>(TimeZoneInfo.ConvertTimeFromUtc(From, timezone),
            TimeZoneInfo.ConvertTimeFromUtc(Upto, timezone), info);
    }

    public override string ToString()
    {
        return $"{Info} - {base.ToString()}";
    }
}