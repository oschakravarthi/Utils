using Microsoft.ApplicationInsights;
using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Exposition.Telemetry.Metrics;
using System;
using System.Collections.Generic;
using System.Net;

namespace SubhadraSolutions.Utils.Exposition;

public class MetricsSupportedExpositionLookup(string apiBaseUrl, TelemetryClient telemetryClient = null) : ExpositionLookup(apiBaseUrl)
{
    private readonly object synclock = new();

    private bool metricsRegistered;

    public ExpositionLookupMetrics Metrics { get; private set; }

    public override object Execute(RequestInfo requestInfo, JObject actionArguments)
    {
        if (!metricsRegistered)
        {
            lock (synclock)
            {
                if (!metricsRegistered)
                {
                    RegisterPerformanceCounters();
                    metricsRegistered = true;
                }
            }
        }

        Metrics.RequestReceived(requestInfo);

        Exception exception = null;
        var executionStartTimestamp = GlobalSettings.Instance.DateTimeNow;
        var ticksStart = SharedStopwatch.ElapsedTicks;
        try
        {
            var result = base.Execute(requestInfo, actionArguments);
            return result;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            var ticksTaken = SharedStopwatch.ElapsedTicks - ticksStart;
            AfterExecute(requestInfo, actionArguments, executionStartTimestamp, ticksTaken, exception);
        }
    }

    private void RegisterPerformanceCounters()
    {
        Metrics = new ExpositionLookupMetrics(ApiBaseUrl, lookup.Keys);
    }

    private void AfterExecute(RequestInfo requestInfo, JObject actionArguments, DateTime executionStartTimestamp,
        long ticksTaken, Exception exception)
    {
        Metrics.RequestProcessed(requestInfo, ticksTaken);
        if (telemetryClient == null)
        {
            return;
        }
        var payload = actionArguments.ToString();
        var relativeUri = requestInfo.ToString();
        if (exception == null)
        {
            telemetryClient.TrackRequest(relativeUri, executionStartTimestamp, TimeSpan.FromTicks(ticksTaken), "200",
                true);
        }
        else
        {
            var statusCode = exception is ExpositionException { StatusCode: not null } expositionException
                ? expositionException.StatusCode.Value
                : (int)HttpStatusCode.InternalServerError;
            telemetryClient.TrackRequest(relativeUri, executionStartTimestamp, TimeSpan.FromTicks(ticksTaken),
                statusCode.ToString(), false);
            var properties = new Dictionary<string, string>
            {
                { "Api", relativeUri },
                { "Payload", payload }
            };
            telemetryClient.TrackException(exception, properties);
        }
    }
}