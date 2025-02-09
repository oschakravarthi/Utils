using System;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace SubhadraSolutions.Utils.ServiceModel.Telemetry;

public class MetricsHttpMessageHandlerBehavior : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        bindingParameters.Add(new Func<HttpClientHandler, HttpMessageHandler>(x => new MetricsInterceptingHttpMessageHandler(x)));
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }

    public HttpMessageHandler GetHttpMessageHandler(HttpMessageHandler innerHandler)
    {
        return new MetricsInterceptingHttpMessageHandler(innerHandler);
    }
}