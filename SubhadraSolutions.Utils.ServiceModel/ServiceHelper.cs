using SubhadraSolutions.Utils.Net;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;

namespace SubhadraSolutions.Utils.ServiceModel;

public static class ServiceHelper
{
    public static string GetSoapAction(this HttpRequestMessage requestMessage)
    {
        var soapAction = "Unknown";
        requestMessage.Headers.TryGetValues("SOAPAction", out var vals);
        if (vals?.Any() == true)
        {
            soapAction = vals.First();
            soapAction = soapAction.Trim('"');
            return soapAction;
        }
        if (requestMessage.Content == null)
        {
            return soapAction;
        }
        var content = requestMessage.Content.ReadAsStringAsync().Result;
        if (string.IsNullOrWhiteSpace(content))
        {
            return soapAction;
        }

        try
        {
            using (var sr = new StringReader(content))
            {
                var document = XDocument.Load(sr);
                var header = document.Root.Elements().Where(e => e.Name.LocalName == "Header").FirstOrDefault();
                if (header != null)
                {
                    var action = header.Elements().Where(e => e.Name.LocalName == "Action").FirstOrDefault();
                    if (action != null)
                    {
                        soapAction = action.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }

        return soapAction;
    }

    public static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration, TimeSpan? timeout)
    {
        Binding binding = null;
        if (endpointConfiguration == EndpointConfiguration.Soap)
        {
            binding = new BasicHttpBinding
            {
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true,
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport
                },
            };
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
        }

        if (endpointConfiguration == EndpointConfiguration.Soap12)
        {
            var customBinding = new CustomBinding();
            var textBindingElement = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None)
            };
            customBinding.Elements.Add(textBindingElement);
            var httpsBindingElement = new HttpsTransportBindingElement
            {
                AllowCookies = true,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };
            customBinding.Elements.Add(httpsBindingElement);
            binding = customBinding;
        }
        if (binding == null)
        {
            throw new InvalidOperationException($"Could not find endpoint with name \'{endpointConfiguration}\'.");
        }
        binding.SetTimeout(timeout);
        return binding;
    }

    public static void SetTimeout(this Binding binding, TimeSpan? timeout)
    {
        var tOut = timeout ?? NetDefaults.DefaultTimeout;
        binding.SendTimeout = tOut;
        binding.ReceiveTimeout = tOut;
    }

    public static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration, string url)
    {
        if (endpointConfiguration == EndpointConfiguration.Soap)
        {
            return new EndpointAddress(url);
        }

        if (endpointConfiguration == EndpointConfiguration.Soap12)
        {
            return new EndpointAddress(url);
        }

        throw new InvalidOperationException($"Could not find endpoint with name \'{endpointConfiguration}\'.");
    }
}