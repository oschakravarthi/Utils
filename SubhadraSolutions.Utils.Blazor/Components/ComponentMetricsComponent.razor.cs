using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.Telemetry;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class ComponentMetricsComponent : AbstractSmartComponent
{
    [Parameter] public List<ObjectMetrics> Metrics { get; set; }

    private IList BuildListForDataGrid()
    {
        var result = MetricsHelper.BuildListFromMetricsOfSameComponentType(Metrics);
        return result;
    }
}