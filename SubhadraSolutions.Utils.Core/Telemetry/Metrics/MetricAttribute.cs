using System;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class MetricAttribute : Attribute
{
    public MetricAttribute(MetricType metricType)
    {
        MetricType = metricType;
    }

    public MetricType MetricType { get; set; }
}