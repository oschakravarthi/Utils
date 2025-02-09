using SubhadraSolutions.Utils.Diagnostics;
using System;

namespace SubhadraSolutions.Utils.Telemetry;

[AttributeUsage(AttributeTargets.Method)]
public sealed class InstrumentLogAttribute : Attribute
{
    public InstrumentLogAttribute(MethodLogOption logOption)
    {
        LogOption = logOption;
    }

    public MethodLogOption LogOption { get; }
}