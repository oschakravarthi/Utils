using Microsoft.Extensions.DependencyInjection;
using SubhadraSolutions.Utils.ApiClient.Contracts;
using SubhadraSolutions.Utils.ApiClient.Providers;
using SubhadraSolutions.Utils.Net.Http;
using System.Net;
using System.Net.Http;

namespace SubhadraSolutions.Utils.ApiClient.Hosting;

public static class HostingHelper
{
    public static IHttpClient ConfigureApiClient(this IServiceCollection services, HttpClient httpClient)
    {
        ServicePointManager.DefaultConnectionLimit = 200;

        var httpClientWrapper = new HttpClientWrapper("HttpClientRest", httpClient);
        services.AddSingleton<IHttpClient>(httpClientWrapper);

        var linqDataProvider = new LinqDataProvider(httpClientWrapper);
        services.AddSingleton<ILinqDataProvider>(linqDataProvider);

        return httpClientWrapper;
    }

    public static IServiceCollection ConfigureCommonProviders(this IServiceCollection services, IHttpClient httpClient, string apiBasePath)
    {
        services.AddSingleton<IMetricsProvider>(new MetricsProvider(httpClient,
            $"{apiBasePath}/common/metrics/GetMetrics", $"{apiBasePath}/common/metrics/ResetMetrics"));
        services.AddSingleton<IConfigurationProvider>(new ConfigurationProvider(httpClient,
            $"{apiBasePath}/common/config/GetConfiguration"));

        return services;
    }
}