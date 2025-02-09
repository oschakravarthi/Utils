using System;

namespace SubhadraSolutions.Utils.Data;

public class MeasureInfo(string propertyName, AggregationType aggregationType) : IMeasure, IEquatable<MeasureInfo>,
    IComparable<MeasureInfo>
{
    public AggregationType AggregationType { get; } = aggregationType;

    public string TargetPropertyName
    {
        get
        {
            if (AggregationType == AggregationType.None)
            {
                return PropertyName;
            }

            return AggregationType + "_" + PropertyName;
        }
    }

    public int CompareTo(MeasureInfo other)
    {
        if (other == null)
        {
            return 1;
        }

        var result = ((int)AggregationType).CompareTo((int)other.AggregationType);
        if (result != 0)
        {
            return result;
        }

        return string.Compare(PropertyName, other.PropertyName);
    }

    public bool Equals(MeasureInfo other)
    {
        return CompareTo(other) == 0;
    }

    public string PropertyName { get; } = propertyName;

    public override bool Equals(object obj)
    {
        return Equals(obj as MeasureInfo);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override string ToString()
    {
        if (AggregationType == AggregationType.None)
        {
            return PropertyName;
        }

        return $"{AggregationType.ToString().ToLower()}({PropertyName})";
    }
}