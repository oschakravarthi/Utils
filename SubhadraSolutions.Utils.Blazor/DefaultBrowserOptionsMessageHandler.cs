using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor;

public sealed class DefaultBrowserOptionsMessageHandler : DelegatingHandler
{
    public DefaultBrowserOptionsMessageHandler(HttpMessageHandler innerHandler = null)
    {
        if (innerHandler != null)
        {
            InnerHandler = innerHandler;
        }
    }

    public BrowserRequestCache DefaultBrowserRequestCache { get; set; }
    public BrowserRequestCredentials DefaultBrowserRequestCredentials { get; set; }
    public BrowserRequestMode DefaultBrowserRequestMode { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the existing options to not override them if set explicitly
        IDictionary<string, object> existingProperties = null;
        if (request.Properties.TryGetValue("WebAssemblyFetchOptions", out var fetchOptions))
        {
            existingProperties = (IDictionary<string, object>)fetchOptions;
        }

        if (existingProperties?.ContainsKey("cache") != true)
        {
            request.SetBrowserRequestCache(DefaultBrowserRequestCache);
        }

        if (existingProperties?.ContainsKey("credentials") != true)
        {
            request.SetBrowserRequestCredentials(DefaultBrowserRequestCredentials);
        }

        if (existingProperties?.ContainsKey("mode") != true)
        {
            request.SetBrowserRequestMode(DefaultBrowserRequestMode);
        }

        return base.SendAsync(request, cancellationToken);
    }
}