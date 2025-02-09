using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SortAttribute : Attribute
{
    public SortAttribute(SortingOrder sortDirection)
    {
        SortOrder = sortDirection;
    }

    public SortingOrder SortOrder { get; }
}