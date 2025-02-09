using Newtonsoft.Json;
using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils;

public class DateTimeRange : Range<DateTime>, IToLocal<DateTimeRange>
{
    [JsonConstructor]
    [System.Text.Json.Serialization.JsonConstructor]
    public DateTimeRange(DateTime from, DateTime upto) : base(from, upto)
    {
    }

    public DateTimeRange(DateTime from, TimeSpan duration) : base(from, from + duration)
    {
    }

    public DateTimeRange(DateTimeRange range) : this(range.From, range.Upto)
    {
    }

    public DateTimeRange(Range<DateTime> range) : base(range)
    {
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public TimeSpan Duration => Upto - From;

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime Mid => From.AddTicks(Duration.Ticks / 2);

    public DateTimeRange ToLocal(TimeZoneInfo timezone)
    {
        return new DateTimeRange(TimeZoneInfo.ConvertTimeFromUtc(From, timezone),
            TimeZoneInfo.ConvertTimeFromUtc(Upto, timezone));
    }

    //public bool Contains(DateTime datetime)
    //{
    //    return datetime >= this.From && datetime <= this.Upto;
    //}
    public bool NearEquals(Range<DateTime> other, TimeSpan delta)
    {
        var result = From - other.From <= delta && Upto - other.Upto <= delta;
        return result;
    }

    public IEnumerable<DateTimeRange> Split(uint numberOfChunks)
    {
        var splitDuration = (double)Duration.Ticks / numberOfChunks;
        var from = From;
        for (var i = 0; i < numberOfChunks; i++)
        {
            var upto = from.AddTicks((long)splitDuration);
            yield return new DateTimeRange(from, upto);
            from = upto;
        }
    }

    public override string ToString()
    {
        return
            $"{From.ToString(GlobalSettings.Instance.DateAndTimeFormat)}-{Upto.ToString(GlobalSettings.Instance.DateAndTimeFormat)}";
    }
}