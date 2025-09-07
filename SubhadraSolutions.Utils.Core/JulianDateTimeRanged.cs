using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class JulianDateTimeRanged<I> : JulianDateTimeRange
{
    [Newtonsoft.Json.JsonConstructor]
    [JsonConstructor]
    public JulianDateTimeRanged(JulianDateTime from, JulianDateTime upto, I info) : base(from, upto)
    {
        Info = info;
    }

    public JulianDateTimeRanged(JulianDateTime from, JulianTimeSpan duration, I info) : base(from, from + duration)
    {
        Info = info;
    }

    public JulianDateTimeRanged(Range<JulianDateTime> range, I info) : base(range.From, range.Upto)
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