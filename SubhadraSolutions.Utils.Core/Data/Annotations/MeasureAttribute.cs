using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MeasureAttribute : Attribute
{
    public MeasureAttribute(AggregationTypes aggregationTypes = AggregationTypes.Sum, bool isDefault = false)
    {
        AggregationTypes = aggregationTypes;
        IsDefault = isDefault;
    }

    public AggregationTypes AggregationTypes { get; }
    public bool IsDefault { get; }
}