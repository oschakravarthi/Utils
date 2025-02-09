using SubhadraSolutions.Utils.Contracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubhadraSolutions.Utils.SlidingWindows;

[Serializable]
public abstract class AbstractAggregatedDTO : ITimeRanged
{
    [Column("FromTimestamp")] public DateTime FromTimestamp { get; set; }

    [Column("UptoTimestamp")] public DateTime UptoTimestamp { get; set; }

    public DateTime GetFromTimestamp()
    {
        return FromTimestamp;
    }

    public DateTime GetUptoTimestamp()
    {
        return UptoTimestamp;
    }
}