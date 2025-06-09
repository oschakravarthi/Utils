using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class DateTimeRanged<I> : DateTimeRange
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

    public override string ToString()
    {
        return $"{Info} - {base.ToString()}";
    }
}