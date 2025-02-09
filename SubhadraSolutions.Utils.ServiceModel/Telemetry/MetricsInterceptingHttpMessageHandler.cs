using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.ServiceModel.Telemetry.Metrics;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ServiceModel.Telemetry;

public class MetricsInterceptingHttpMessageHandler : DelegatingHandler
{
    private static readonly ConcurrentDictionary<string, SOAPClientMetrics> _metricsLookup =
        new(StringComparer.OrdinalIgnoreCase);

    public MetricsInterceptingHttpMessageHandler(HttpMessageHandler innerHandler)
    {
        InnerHandler = innerHandler;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var info = SoapRequestInfo.BuildFromRequestMessage(request);
        var metrics = _metricsLookup.GetOrAdd(info.Uri, key =>
        {
            var m = new SOAPClientMetrics(key);
            return m;
        });
        var actionKey = info.BuildKey();
        metrics.RequestReceived(actionKey);
        var ticksStart = SharedStopwatch.ElapsedTicks;
        var result = base.Send(request, cancellationToken);
        metrics.RequestProcessed(actionKey, SharedStopwatch.ElapsedTicks - ticksStart);
        return result;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var info = SoapRequestInfo.BuildFromRequestMessage(request);
        var metrics = _metricsLookup.GetOrAdd(info.Uri, k =>
        {
            var m = new SOAPClientMetrics(k);
            return m;
        });
        var actionKey = info.BuildKey();
        metrics.RequestReceived(actionKey);
        var ticksStart = SharedStopwatch.ElapsedTicks;
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        metrics.RequestProcessed(actionKey, SharedStopwatch.ElapsedTicks - ticksStart);

        return response;
    }

    private class SoapRequestInfo(string uri, string soapAction)
    {
        public string SOAPAction { get; } = soapAction;
        public string Uri { get; } = uri;

        public static SoapRequestInfo BuildFromRequestMessage(HttpRequestMessage requestMessage)
        {
            var soapAction = requestMessage.GetSoapAction();
            var uri = requestMessage.RequestUri.GetLeftPart(UriPartial.Path);
            return new SoapRequestInfo(uri, soapAction);
        }

        public string BuildKey()
        {
            //var trimmedSoapAction = SOAPAction;
            //var index = trimmedSoapAction.LastIndexOf('/');
            //if (index > -1)
            //{
            //    trimmedSoapAction = trimmedSoapAction.Substring(index + 1);
            //}

            //return $"{Uri}/{trimmedSoapAction}";

            return $"{this.Uri} - {SOAPAction}";
        }
    }
}