using SubhadraSolutions.Utils.Telemetry.Metrics;

namespace SubhadraSolutions.Utils.ServiceModel.Telemetry.Metrics;

public class SOAPClientMetrics(string parentKey) : AbstractCompositeRequestProcessingMetrics<string, SOAPActionMetrics>(parentKey)
{
    protected override SOAPActionMetrics BuildMetrics(string request)
    {
        return new SOAPActionMetrics(request);
    }
}