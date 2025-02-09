using SubhadraSolutions.Utils.Contracts;
using System;
using System.Threading;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public class MetricsTrackerMetrics : IResetable
{
    private long _numberofTimesMetricsCaptured;
    private long _ticksTaken;

    public long AverageTicksTakenPerCapture
    {
        get
        {
            var count = Interlocked.Read(ref _numberofTimesMetricsCaptured);
            if (count == 0)
            {
                return 0;
            }

            return Interlocked.Read(ref _ticksTaken) / count;
        }
    }

    public DateTime? LastCapturedOn { get; private set; }

    public long NumberofTimesMetricsCaptured => Interlocked.Read(ref _numberofTimesMetricsCaptured);

    public long TicksTaken => Interlocked.Read(ref _ticksTaken);

    internal void MetricsCaptured(long ticksTaken)
    {
        LastCapturedOn = GeneralHelper.CurrentDateTime;
        Interlocked.Increment(ref _numberofTimesMetricsCaptured);
        Interlocked.Add(ref _ticksTaken, ticksTaken);
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _numberofTimesMetricsCaptured, 0);
        Interlocked.Exchange(ref _ticksTaken, 0);
    }
}