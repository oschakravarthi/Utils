using SubhadraSolutions.Utils.ApiClient.Contracts;
using SubhadraSolutions.Utils.Net.Http;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Providers;

public class MetricsProvider : AbstractProvider, IMetricsProvider
{
    private readonly string resetApiPath;

    public MetricsProvider(IHttpClient httpClient, string getApiPath, string resetApiPath)
        : base(httpClient, getApiPath)
    {
        this.resetApiPath = resetApiPath;
    }

    public Task<List<ObjectMetrics>> GetMetricsAsync()
    {
        return GetAsync<List<ObjectMetrics>>();
    }

    public async Task ResetMetricsAsync()
    {
        await httpClient.DeleteAsync(this.resetApiPath).ConfigureAwait(false);
    }
}