using System;
using System.Net.Http;

namespace SubhadraSolutions.Utils.Blazor.Helpers;

public static class HttpClientHelper
{
    public static HttpClient BuildHttpClient(string baseAddress, TimeSpan timeout, HttpMessageHandler handler = null)
    {
        if (handler == null)
        {
            handler = new HttpClientHandler();
        }
        //ServicePointManager.DefaultConnectionLimit = 200;
        //var httpClient = new HttpClient(new DefaultBrowserOptionsMessageHandler(handler)
        //{
        //    DefaultBrowserRequestCache = BrowserRequestCache.NoStore,
        //    DefaultBrowserRequestCredentials = BrowserRequestCredentials.Include,
        //    DefaultBrowserRequestMode = BrowserRequestMode.Cors
        //})
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress),
            Timeout = timeout
        };

        return httpClient;
    }
}