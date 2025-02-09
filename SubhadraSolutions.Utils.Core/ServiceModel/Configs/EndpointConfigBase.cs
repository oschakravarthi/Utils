using System;

namespace SubhadraSolutions.Utils.ServiceModel.Configs
{
    public abstract class EndpointConfigBase
    {
        public TimeSpan? Timeout { get; set; }
    }
}