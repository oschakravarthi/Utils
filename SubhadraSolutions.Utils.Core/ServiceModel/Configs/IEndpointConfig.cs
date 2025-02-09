using System;

namespace SubhadraSolutions.Utils.ServiceModel.Configs;

public interface IEndpointConfig
{
    TimeSpan? Timeout { get; }
    string Uri { get; }
}