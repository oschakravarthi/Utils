using SubhadraSolutions.Utils.Contracts;
using System.Diagnostics;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

[DebuggerDisplay("{Name}, {Value}")]
public struct Metric : INamed
{
    public Metric(string name, double value, MetricType metricType)
    {
        this.Name = name;
        this.Value = value;
        this.MetricType = metricType;
    }

    public string Name { get; }
    public double Value { get; }
    public MetricType MetricType { get; }
}