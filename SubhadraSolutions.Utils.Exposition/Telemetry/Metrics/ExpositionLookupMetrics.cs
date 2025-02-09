using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Exposition.Telemetry.Metrics;

public class ExpositionLookupMetrics : AbstractCompositeRequestProcessingMetrics<RequestInfo, APIMetrics>, IResetable
{
    public ExpositionLookupMetrics(string name, IEnumerable<RequestInfo> requestInfos) : base(name)
    {
        foreach (var requestInfo in requestInfos)
        {
            GetOrAddMetric(requestInfo);
        }
    }

    protected override APIMetrics BuildMetrics(RequestInfo request)
    {
        var key = BuildKeyForAction(Name, request);
        var apiMetrics = new APIMetrics(key);
        return apiMetrics;
    }

    private static string BuildKeyForAction(string parentName, RequestInfo requestInfo)
    {
        var s = requestInfo.HttpRequestMethod.ToString().ToUpper() + " " + parentName;
        if (requestInfo.Path != null)
        {
            s += "/" + requestInfo.Path;
        }

        s += "/" + requestInfo.ActionName;
        return s;
    }

    public void Reset()
    {
        ResetCore();
    }
}