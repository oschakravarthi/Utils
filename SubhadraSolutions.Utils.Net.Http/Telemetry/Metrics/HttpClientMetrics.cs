using SubhadraSolutions.Utils.Telemetry.Metrics;

namespace SubhadraSolutions.Utils.Net.Http.Telemetry.Metrics;

public class HttpClientMetrics : RequestProcessingMetrics
{
    public HttpClientMetrics(string name) : base(name)
    {
    }
}