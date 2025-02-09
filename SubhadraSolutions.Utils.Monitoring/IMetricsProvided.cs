using System;

namespace SubhadraSolutions.Utils.Monitoring;

public interface IMetricsProvided
{
    public event EventHandler<MetricsEventArgs> OnMetrics;
}