using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class KPIAttribute : Attribute
{
    public KPIAttribute(bool isPercentage, bool? isPositiveValuePositive = null)
    {
        IsPercentage = isPercentage;
        IsPositiveValuePositive = isPositiveValuePositive;
    }

    public bool IsPercentage { get; }
    public bool? IsPositiveValuePositive { get; private set; }
}