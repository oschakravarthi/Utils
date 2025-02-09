using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Monitoring;

public class MetricsEventArgs(IReadOnlyList<MetricSeries> metricSeriesList) : EventArgs
{
    public IReadOnlyList<MetricSeries> MetricSeriesList { get; } = metricSeriesList;
}