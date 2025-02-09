using SubhadraSolutions.Utils.Data;
using System;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public class ExtendedMeasureInfo(string measurePropertyName, AggregationType aggregationType,
        ExtendedApexSeriesType seriesType)
    : MeasureInfo(measurePropertyName, aggregationType), IEquatable<ExtendedMeasureInfo>, IComparable<ExtendedMeasureInfo>
{
    public ExtendedMeasureInfo(string measurePropertyName, AggregationType aggregationType) : this(measurePropertyName,
        aggregationType, ExtendedApexSeriesType.Auto)
    {
    }

    public ExtendedApexSeriesType SeriesType { get; } = seriesType;

    public int CompareTo(ExtendedMeasureInfo other)
    {
        return base.CompareTo(other);
    }

    public bool Equals(ExtendedMeasureInfo other)
    {
        return base.Equals(other);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ExtendedMeasureInfo);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}