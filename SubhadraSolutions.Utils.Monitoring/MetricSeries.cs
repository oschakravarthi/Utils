using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Monitoring;

public class MetricSeries(string name, IReadOnlyList<Metric> metrics)
{
    public IReadOnlyList<Metric> Metrics { get; } = metrics;
    public string Name { get; } = name;
}