using SubhadraSolutions.Utils.Contracts;
using System.Threading;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public abstract class RequestProcessingMetricsBase : INamed
{
    //private long _totalNumberOfRequestsReceived;
    //public long _totalNumberOfRequestsProcessed;
    //public long _totalTicksTaken;

    private long _numberOfRequestsProcessed;
    private long _numberOfRequestsReceived;
    private long _ticksTaken;

    protected RequestProcessingMetricsBase(string name)
    {
        Name = name;
        MetricsTracker.Instance.Register(name, this);
    }

    [Metric(MetricType.Snapshot)]
    public long AverageTicksTakenPerRequest
    {
        get
        {
            var requestsServed = NumberOfRequestsProcessed;
            if (requestsServed == 0)
            {
                return 0;
            }

            var ticks = TicksTaken;
            return ticks / requestsServed;
        }
    }

    //[Metric(MetricAggregationType.Latest)]
    //public long TotalNumberOfRequestsReceived
    //{
    //    get { return Interlocked.Read(ref _totalNumberOfRequestsReceived); }
    //}

    //[Metric(MetricAggregationType.Latest)]
    //public long TotalNumberOfRequestsProcessed
    //{
    //    get { return Interlocked.Read(ref _totalNumberOfRequestsProcessed); }
    //}

    //[Metric(MetricAggregationType.Latest)]
    //public long TotalNumberOfRequestsInProcess
    //{
    //    get { return _totalNumberOfRequestsReceived - TotalNumberOfRequestsProcessed; }
    //}

    //[Metric(MetricAggregationType.Latest)]
    //public long TotalTicksTaken
    //{
    //    get { return Interlocked.Read(ref _totalTicksTaken); }
    //}

    //[Metric(MetricAggregationType.Latest)]
    //public long TotalAverageTicksTakenPerRequest
    //{
    //    get
    //    {
    //        var requestsProcessed = TotalNumberOfRequestsProcessed;
    //        if (requestsProcessed == 0)
    //        {
    //            return 0;
    //        }
    //        var totalTicks = TotalTicksTaken;
    //        return totalTicks / requestsProcessed;
    //    }
    //}

    [Metric(MetricType.Snapshot)]
    public long NumberOfRequestsProcessed => Interlocked.Read(ref _numberOfRequestsProcessed);

    [Metric(MetricType.Snapshot)]
    public long NumberOfRequestsReceived => Interlocked.Read(ref _numberOfRequestsReceived);

    [Metric(MetricType.Snapshot)]
    public long TicksTaken => Interlocked.Read(ref _ticksTaken);

    public string Name { get; }

    protected void RequestProcessedCore(long ticksTaken)
    {
        //Interlocked.Increment(ref _totalNumberOfRequestsProcessed);
        Interlocked.Increment(ref _numberOfRequestsProcessed);
        Interlocked.Add(ref _ticksTaken, ticksTaken);
        //Interlocked.Add(ref _totalTicksTaken, ticksTaken);
    }

    protected void RequestReceivedCore()
    {
        //Interlocked.Increment(ref _totalNumberOfRequestsReceived);
        Interlocked.Increment(ref _numberOfRequestsReceived);
    }

    protected void ResetCore()
    {
        Interlocked.Exchange(ref _numberOfRequestsReceived, 0);
        Interlocked.Exchange(ref _numberOfRequestsProcessed, 0);
        Interlocked.Exchange(ref _ticksTaken, 0);
    }
}