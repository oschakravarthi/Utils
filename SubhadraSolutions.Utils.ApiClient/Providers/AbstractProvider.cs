using SubhadraSolutions.Utils.Net.Http;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Providers;

public abstract class AbstractProvider
{
    protected readonly string apiPath;
    protected readonly IHttpClient httpClient;

    protected AbstractProvider(IHttpClient httpClient, string apiPath)
    {
        this.httpClient = httpClient;
        this.apiPath = apiPath;
    }

    protected async Task<TResult> GetAsync<TResult>()
    {
        return await httpClient.GetAsync<TResult>(apiPath).ConfigureAwait(false);
    }
}