using SubhadraSolutions.Utils.Contracts;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public class RequestProcessingMetrics : RequestProcessingMetricsBase, IResetable
{
    public RequestProcessingMetrics(string name) : base(name)
    {
    }

    public void RequestProcessed(long ticksTaken)
    {
        RequestProcessedCore(ticksTaken);
    }

    public void RequestReceived()
    {
        RequestReceivedCore();
    }

    public void Reset()
    {
        ResetCore();
    }
}