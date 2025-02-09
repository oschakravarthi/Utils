using System;

namespace SubhadraSolutions.Utils.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigurableAttribute : Attribute
    {
    }
}