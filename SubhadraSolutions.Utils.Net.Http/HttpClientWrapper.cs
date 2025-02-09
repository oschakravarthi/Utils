using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Identity;
using SubhadraSolutions.Utils.Net.Http.Telemetry.Metrics;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http;

public class HttpClientWrapper : AbstractHttpClient, IHttpClient
{
    private readonly HttpClient _httpClient;

    private readonly HttpClientMetrics _metrics;

    private readonly ITokenProvider tokenProvider;

    public HttpClientWrapper(string name, HttpClient httpClient, ITokenProvider tokenProvider = null)
    {
        Name = name;
        _metrics = new HttpClientMetrics(name);
        _httpClient = httpClient;
        this.tokenProvider = tokenProvider;
    }

    public override Uri BaseAddress => this._httpClient.BaseAddress;
    public RequestProcessingMetrics Metrics => _metrics;

    public string Name { get; }

    public override async Task DownloadFileAsync(string url, string pathToSave, CancellationToken? cancellationToken)
    {
        _metrics.RequestReceived();
        var ticksStart = SharedStopwatch.ElapsedTicks;
        await _httpClient.DownloadFileAsync(url, pathToSave, cancellationToken).ConfigureAwait(false);
        var ticksTaken = SharedStopwatch.ElapsedTicks - ticksStart;
        _metrics.RequestProcessed(ticksTaken);
    }

    protected override async ValueTask AddHeadersAsync(HttpRequestMessage requestMessage, IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        await base.AddHeadersAsync(requestMessage, headers, cancellationToken).ConfigureAwait(false);
        if (tokenProvider != null)
        {
            var token = await tokenProvider.GetTokenAsync(cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType ?? "Bearer", token.AccessToken);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _httpClient?.Dispose();
    }

    protected override async ValueTask<HttpResponseMessage> SendCoreAsync(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null)
    {
        _metrics.RequestReceived();
        var ticksStart = SharedStopwatch.ElapsedTicks;
        var response = await _httpClient.SendAsync(requestMessage,
        cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        var ticksTaken = SharedStopwatch.ElapsedTicks - ticksStart;
        _metrics.RequestProcessed(ticksTaken);
        return response;
    }
}