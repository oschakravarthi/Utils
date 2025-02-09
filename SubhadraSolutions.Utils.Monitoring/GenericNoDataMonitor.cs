using SubhadraSolutions.Utils.SlidingWindows;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;

namespace SubhadraSolutions.Utils.Monitoring;

public class GenericNoDataMonitor<T> : AbstractMonitor<T>
{
    private readonly int numberOfFetchesToCheck;

    public GenericNoDataMonitor(ISlidingWindowCache<T> slidingWindowCache, int numberOfFetchesToCheck)
        : base(slidingWindowCache)
    {
        this.numberOfFetchesToCheck = numberOfFetchesToCheck;
        Name = $"{typeof(T)}_NoData";

        slidingWindowCache.OnDataFetch += SlidingWindowCache_OnDataFetch;
    }

    public int NumberOfContinuousFetchesWithNoData { get; private set; }

    protected override AlertCheckResult CheckForAlertCore()
    {
        if (NumberOfContinuousFetchesWithNoData >= numberOfFetchesToCheck)
        {
            return new AlertCheckResult(AlertStatus.On);
        }

        return new AlertCheckResult(AlertStatus.Off);
    }

    private void SlidingWindowCache_OnDataFetch(object sender, DataFetchEventArgs<T> e)
    {
        if (e.Window.Items.Count == 0)
        {
            NumberOfContinuousFetchesWithNoData++;
        }
        else
        {
            NumberOfContinuousFetchesWithNoData = 0;
        }
    }
}