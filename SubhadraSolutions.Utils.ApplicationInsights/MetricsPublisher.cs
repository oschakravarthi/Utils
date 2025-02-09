using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using SubhadraSolutions.Utils.Telemetry.Metrics;

namespace SubhadraSolutions.Utils.ApplicationInsights;

public class MetricsPublisher
{
    private readonly TelemetryClient telemetryClient;

    public MetricsPublisher(TelemetryClient telemetryClient)
    {
        this.telemetryClient = telemetryClient;
        MetricsTracker.Instance.OnMetricsCaptured += MetricsTracker_OnMetricsCaptured;
    }

    //private void MetricsTracker_OnMetricsCaptured()
    //{
    //    //var now = GlobalSettings.Instance.DateTimeNow;
    //    var e = MetricsTracker.Instance.GetMetrics();
    //    foreach (var kvp in e)
    //    {
    //        var metrics = kvp.Metrics;
    //        for (int i = 0; i < metrics.Length; i++)
    //        {
    //            var metric = metrics[i];
    //            var identifier = new MetricIdentifier(kvp.ObjectName,)
    //            {
    //            }
    //            var telemetryMetric = this.telemetryClient.GetMetric(metric.Name, nameof(kvp.ObjectType), nameof(kvp.ObjectName));
    //            telemetryMetric.Identifier.MetricNamespace
    //            telemetryMetric.TrackValue(metric.Value, kvp.ObjectType, kvp.ObjectName);
    //        }
    //    }
    //    //this.telemetryClient.Flush();
    //}

    private void MetricsTracker_OnMetricsCaptured()
    {
        var now = GlobalSettings.Instance.DateTimeNow;
        var e = MetricsTracker.Instance.GetMetrics();
        foreach (var kvp in e)
        {
            var metrics = kvp.Metrics;
            for (var i = 0; i < metrics.Length; i++)
            {
                var metric = metrics[i];
                if (metric.MetricType != MetricType.Snapshot)
                {
                    continue;
                }
                var telemetry = new MetricTelemetry
                {
                    MetricNamespace = $"{kvp.ObjectType}:{kvp.ObjectName}",
                    Name = metric.Name,
                    Sum = metric.Value,
                    Timestamp = now,
                };
                //var m = telemetryClient.GetMetric(metric.Name, kvp.ObjectType, kvp.ObjectName);
                telemetryClient.TrackMetric(telemetry);
            }
        }

        telemetryClient.Flush();
    }
}