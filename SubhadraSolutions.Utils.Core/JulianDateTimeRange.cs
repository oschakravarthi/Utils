using Newtonsoft.Json;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils;

public class JulianDateTimeRange : Range<JulianDateTime>
{
    [JsonConstructor]
    [System.Text.Json.Serialization.JsonConstructor]
    public JulianDateTimeRange(JulianDateTime from, JulianDateTime upto) : base(from, upto)
    {
    }

    public JulianDateTimeRange(JulianDateTime from, JulianTimeSpan duration) : base(from, from + duration)
    {
    }

    public JulianDateTimeRange(JulianDateTimeRange range) : this(range.From, range.Upto)
    {
    }

    public JulianDateTimeRange(Range<JulianDateTime> range) : base(range)
    {
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public JulianTimeSpan Duration => Upto - From;

    [System.Text.Json.Serialization.JsonIgnore]
    public JulianDateTime Mid => From+(Duration / 2);

    //public bool Contains(DateTime datetime)
    //{
    //    return datetime >= this.From && datetime <= this.Upto;
    //}
    public bool NearEquals(Range<JulianDateTime> other, JulianTimeSpan delta)
    {
        var result = From - other.From <= delta && Upto - other.Upto <= delta;
        return result;
    }

    public IEnumerable<JulianDateTimeRange> Split(uint numberOfChunks)
    {
        var splitDuration = Duration / numberOfChunks;
        var from = From;
        for (var i = 0; i < numberOfChunks; i++)
        {
            var upto = from+splitDuration;
            yield return new JulianDateTimeRange(from, upto);
            from = upto;
        }
    }

    public override string ToString()
    {
        return $"{From.ToString()}-{Upto.ToString()}";
    }
}