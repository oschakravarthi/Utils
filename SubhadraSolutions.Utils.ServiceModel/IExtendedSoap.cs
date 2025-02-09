using System;

namespace SubhadraSolutions.Utils.ServiceModel;

public interface IExtendedSoap
{
    TimeSpan Timeout { get; set; }
    string Url { get; set; }
}