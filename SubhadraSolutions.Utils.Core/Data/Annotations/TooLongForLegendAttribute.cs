using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TooLongForLegendAttribute : Attribute
{
}