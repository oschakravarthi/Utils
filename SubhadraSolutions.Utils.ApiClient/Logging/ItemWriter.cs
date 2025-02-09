using SubhadraSolutions.Utils.Logging;
using SubhadraSolutions.Utils.Net.Http;
using System;

namespace SubhadraSolutions.Utils.ApiClient.Logging;

public class ItemWriter<T> : IItemWriter<T>
{
    private readonly IHttpClient httpClient;
    private readonly string restApiEndpoint;

    public ItemWriter(IHttpClient httpClient, string restApiEndpoint)
    {
        this.httpClient = httpClient;
        this.restApiEndpoint = restApiEndpoint;
    }

    public async void Write(T item)
    {
        Console.WriteLine(item.ToString());
        try
        {
            await httpClient.PostAsync(restApiEndpoint, item).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}