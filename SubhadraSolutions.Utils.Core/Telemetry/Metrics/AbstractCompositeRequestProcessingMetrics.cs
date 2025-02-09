using System.Collections.Concurrent;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public abstract class AbstractCompositeRequestProcessingMetrics<TRequest, TMetrics> : RequestProcessingMetricsBase
    where TMetrics : RequestProcessingMetrics
{
    private readonly ConcurrentDictionary<TRequest, TMetrics> dictionary = new();

    protected AbstractCompositeRequestProcessingMetrics(string parentKey) : base(parentKey)
    {
    }

    public void RequestProcessed(TRequest request, long ticksTaken)
    {
        RequestProcessedCore(ticksTaken);
        if (dictionary.TryGetValue(request, out var apiMetrics))
        {
            apiMetrics.RequestProcessed(ticksTaken);
        }
    }

    public void RequestReceived(TRequest request)
    {
        RequestReceivedCore();
        var metrics = dictionary.GetOrAdd(request, BuildMetrics);
        metrics.RequestReceived();
    }

    protected abstract TMetrics BuildMetrics(TRequest request);

    protected TMetrics GetOrAddMetric(TRequest request)
    {
        return dictionary.GetOrAdd(request, BuildMetrics);
    }
}