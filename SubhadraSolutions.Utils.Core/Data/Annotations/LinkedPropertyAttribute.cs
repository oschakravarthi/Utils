using System;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class LinkedPropertyAttribute : Attribute
{
    public LinkedPropertyAttribute(string linkedPropertyName)
    {
        LinkedPropertyName = linkedPropertyName;
    }

    public string LinkedPropertyName { get; }
}