using SubhadraSolutions.Utils.ServiceModel.Telemetry;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SubhadraSolutions.Utils.ServiceModel;

public abstract class ExtendedClientBase<TChannel> : ClientBase<TChannel>, IExtendedSoap where TChannel : class
{
    private static readonly MetricsHttpMessageHandlerBehavior _metricsHttpMessageHandlerBehavior = new();

    static ExtendedClientBase()
    {
        //CacheSetting = CacheSetting.AlwaysOn;
        ServicePointManager.ServerCertificateValidationCallback +=
            (se, cert, chain, sslerror) => true;
    }

    protected ExtendedClientBase(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
    {
        if (ClientBaseSettings.CaptureMetrics)
        {
            Endpoint.EndpointBehaviors.Add(_metricsHttpMessageHandlerBehavior);
        }
    }

    public TimeSpan Timeout
    {
        get
        {
            var st = Endpoint.Binding.SendTimeout;
            var rt = Endpoint.Binding.ReceiveTimeout;
            return st > rt ? st : rt;
        }
        set
        {
            Endpoint.Binding.SendTimeout = value;
            Endpoint.Binding.ReceiveTimeout = value;
        }
    }

    public string Url
    {
        get => Endpoint.Address.Uri.ToString();
        set => Endpoint.Address = new EndpointAddress(value);
    }
}