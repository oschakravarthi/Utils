using SubhadraSolutions.Utils.Data;
using System;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public class MeasureAndSeriesType(string propertyName, ExtendedApexSeriesType seriesType) : IMeasure, IComparable<MeasureAndSeriesType>
{
    public ExtendedApexSeriesType SeriesType { get; } = seriesType;

    public int CompareTo(MeasureAndSeriesType other)
    {
        if (other == null)
        {
            return 1;
        }

        var result = string.Compare(PropertyName, other.PropertyName);
        if (result != 0)
        {
            return result;
        }

        return SeriesType.CompareTo(other.SeriesType);
    }

    public string PropertyName { get; } = propertyName;

    public static bool operator !=(MeasureAndSeriesType left, MeasureAndSeriesType right)
    {
        return !(left == right);
    }

    public static bool operator <(MeasureAndSeriesType left, MeasureAndSeriesType right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(MeasureAndSeriesType left, MeasureAndSeriesType right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator ==(MeasureAndSeriesType left, MeasureAndSeriesType right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator >(MeasureAndSeriesType left, MeasureAndSeriesType right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(MeasureAndSeriesType left, MeasureAndSeriesType right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        return CompareTo(obj as MeasureAndSeriesType) == 0;
    }

    public override int GetHashCode()
    {
        return (PropertyName + "\t" + SeriesType).GetHashCode();
    }
}