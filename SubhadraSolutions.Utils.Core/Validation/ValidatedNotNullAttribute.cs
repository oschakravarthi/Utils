namespace SubhadraSolutions.Utils.Validation
{
    using System;

    /// <summary>
    /// The not null validation attribute.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}