using System;

namespace SubhadraSolutions.Utils.Instrumentation;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class InstrumentAttribute(bool enableInstrumentation) : Attribute
{
    public bool EnableInstrumentation { get; } = enableInstrumentation;
}