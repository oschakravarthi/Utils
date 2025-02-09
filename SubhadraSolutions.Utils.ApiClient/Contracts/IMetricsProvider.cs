using SubhadraSolutions.Utils.Telemetry.Metrics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Contracts;

public interface IMetricsProvider
{
    Task<List<ObjectMetrics>> GetMetricsAsync();

    Task ResetMetricsAsync();
}