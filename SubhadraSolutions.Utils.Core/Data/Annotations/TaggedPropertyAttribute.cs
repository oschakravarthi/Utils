using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class TaggedPropertyAttribute : Attribute
{
}