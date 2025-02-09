using SubhadraSolutions.Utils.ApiClient.Contracts;
using SubhadraSolutions.Utils.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Providers;

public class ConfigurationProvider : AbstractProvider, IConfigurationProvider
{
    public ConfigurationProvider(IHttpClient httpClient, string apiPath)
        : base(httpClient, apiPath)
    {
    }

    public Task<List<KeyValuePair<string, string>>> GetConfigurationAsync()
    {
        return GetAsync<List<KeyValuePair<string, string>>>();
    }
}