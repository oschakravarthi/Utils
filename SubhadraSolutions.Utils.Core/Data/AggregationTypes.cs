using System;

namespace SubhadraSolutions.Utils.Data;

[Flags]
public enum AggregationTypes
{
    None = 0,
    Sum = 1,
    Min = 2,
    Max = 4,
    Average = 8,

    //Count = 16,
    DistinctCount = 16,

    All = 31
}