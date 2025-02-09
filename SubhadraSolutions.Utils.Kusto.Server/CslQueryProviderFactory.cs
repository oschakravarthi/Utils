using Azure.Core;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using SubhadraSolutions.Utils.Azure.Identity.Config;
using SubhadraSolutions.Utils.Kusto.Server.RateLimited;
using SubhadraSolutions.Utils.Net.Http.Configs;

namespace SubhadraSolutions.Utils.Kusto.Server;

public class CslQueryProviderFactory(KustoConnectionStringBuilder connectionStringBuilder)
{
    public string DatabaseName { get; } = connectionStringBuilder.InitialCatalog;

    public string Cluster { get; } = connectionStringBuilder.Hostname;

    public ICslQueryProvider CreateCslQueryProvider()
    {
        return KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder);
    }

    public IKustoIngestClient CreateDirectIngestClient()
    {
        return KustoIngestFactory.CreateDirectIngestClient(connectionStringBuilder);
    }

    public IKustoIngestClient CreateStreamingIngestClient()
    {
        return KustoIngestFactory.CreateStreamingIngestClient(connectionStringBuilder);
    }

    public ICslAdminProvider CreateCslAdminProvider()
    {
        return KustoClientFactory.CreateCslAdminProvider(connectionStringBuilder);
    }

    public static ICslQueryProvider BuildCslQueryProviderWithUserManagedIdentity(
        string userManagedIdentity, string kustoConnectionString, RateLimiterWrapperConfig rateLimiterWrapperConfig = null)
    {
        var kcsb = new KustoConnectionStringBuilder(kustoConnectionString)
            .WithAadUserManagedIdentity(userManagedIdentity);
        return BuildCslQueryProvider(kcsb, rateLimiterWrapperConfig);
    }

    public static ICslQueryProvider BuildCslQueryWithAadSystemManagedIdentity(
        string kustoConnectionString, RateLimiterWrapperConfig rateLimiterWrapperConfig = null)
    {
        var kcsb = new KustoConnectionStringBuilder(kustoConnectionString)
            .WithAadSystemManagedIdentity();
        return BuildCslQueryProvider(kcsb, rateLimiterWrapperConfig);
    }

    public static ICslQueryProvider BuildCslQueryProvider(
        IdentityConfigWithSecret identityConfig, string kustoConnectionString, RateLimiterWrapperConfig rateLimiterWrapperConfig = null)
    {
        var key = identityConfig.Secret.GetSecret();
        var kcsb = new KustoConnectionStringBuilder(kustoConnectionString)
            .WithAadApplicationKeyAuthentication(identityConfig.ClientId, key.SecureStringToString(),
                identityConfig.TenantId);
        return BuildCslQueryProvider(kcsb, rateLimiterWrapperConfig);
    }

    public static ICslQueryProvider BuildCslQueryProvider(
        TokenCredential tokenCredential, string kustoConnectionString, RateLimiterWrapperConfig rateLimiterWrapperConfig = null)
    {
        var kcsb = new KustoConnectionStringBuilder(kustoConnectionString)
            .WithAadAzureTokenCredentialsAuthentication(tokenCredential);
        return BuildCslQueryProvider(kcsb, rateLimiterWrapperConfig);
    }

    public static ICslQueryProvider BuildCslQueryProvider(KustoConnectionStringBuilder kcsb, RateLimiterWrapperConfig rateLimiterWrapperConfig = null)
    {
        ICslQueryProvider provider = KustoClientFactory.CreateCslQueryProvider(kcsb); ;
        if (rateLimiterWrapperConfig != null)
        {
            var rateLimiterWrapper = rateLimiterWrapperConfig.BuildRateLimiterWrapper();
            if (rateLimiterWrapper != null)
            {
                provider = new CslQueryProviderRateLimitDecorator(provider, rateLimiterWrapper);
            }
        }
        return provider;
    }
}