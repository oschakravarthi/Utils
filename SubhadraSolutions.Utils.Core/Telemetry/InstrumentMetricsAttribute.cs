using System;

namespace SubhadraSolutions.Utils.Telemetry;

[AttributeUsage(AttributeTargets.Method)]
public sealed class InstrumentMetricsAttribute : Attribute
{
}