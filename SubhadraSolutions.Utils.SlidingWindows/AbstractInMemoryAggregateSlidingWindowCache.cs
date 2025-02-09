using SubhadraSolutions.Utils.Collections.Generic;
using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.SlidingWindows;

public abstract class AbstractInMemoryAggregateSlidingWindowCache<TFrom, TTo> : IAggregateSlidingWindowCache<TTo>
    where TTo : ITimeRanged
{
    private readonly TimeRangedLinkedList<TTo> aggregatedCache;
    protected readonly Func<Window<TFrom>, IReadOnlyList<TTo>> aggregator;
    private readonly ISlidingWindowCache<TFrom> cache;

    protected AbstractInMemoryAggregateSlidingWindowCache(ISlidingWindowCache<TFrom> cache,
        Func<Window<TFrom>, IReadOnlyList<TTo>> aggregator, TimeSpan seriesDuration)
    {
        this.cache = cache;
        this.aggregator = aggregator;
        SeriesDuration = seriesDuration;
        aggregatedCache = [];

        Initialize();
    }

    public event EventHandler<DataFetchEventArgs<TTo>> OnDataFetch;

    public TimeSpan FetchWindowDuration => cache.FetchWindowDuration;

    public DateTime LastFetchTimestamp => cache.LastFetchTimestamp;

    public DateTime MaxTimestamp
    {
        get
        {
            var first = aggregatedCache.Latest;
            return first == null ? cache.MaxTimestamp : first.GetUptoTimestamp();
        }
    }

    public string Name { get; set; }

    public TimeSpan SeriesDuration { get; }

    public IQueryable<TTo> GetAllItems()
    {
        return aggregatedCache.AsQueryable();
    }

    public IReadOnlyList<TTo> GetData(DateTime from, DateTime upto)
    {
        return aggregatedCache.GetWindow(from, upto).ToList().AsReadOnly();
    }

    public IEnumerable<Window<TTo>> GetWindows(TimeSpan windowDuration)
    {
        return WindowingHelper.SplitIntoWindows(aggregatedCache, windowDuration);
    }

    public IEnumerable<Window<TTo>> GetWindows(TimeSpan windowDuration, TimeSpan jumpDuration)
    {
        return WindowingHelper.SplitIntoWindows(aggregatedCache, windowDuration, jumpDuration);
    }

    public IEnumerable<Window<TTo>> GetWindows(TimeSpan windowDuration, uint windowsToSkip, uint maxWindowsToFetch)
    {
        return WindowingHelper.SplitIntoWindows(aggregatedCache, windowDuration, windowsToSkip, maxWindowsToFetch);
    }

    private void AggregateWindow(Window<TFrom> window)
    {
        if (window.Items.Count > 0)
        {
            var aggregated = aggregator(window);
            aggregatedCache.Add(aggregated);
            PurgeExpiredItems();
            FireOnFetch(window.From, window.Upto, aggregated);
        }
        else
        {
            FireOnFetch(window.From, window.Upto, new List<TTo>().AsReadOnly());
        }
    }

    private void FireOnFetch(DateTime from, DateTime upto, IReadOnlyList<TTo> items)
    {
        var onDataFetch = OnDataFetch;
        if (onDataFetch != null)
        {
            var window = new Window<TTo>(from, upto, items);
            var args = new DataFetchEventArgs<TTo>(window, MaxTimestamp);
            onDataFetch(this, args);
        }
    }

    private void Initialize()
    {
        cache.OnDataFetch += (s, e) =>
        {
            lock (this)
            {
                AggregateWindow(e.Window);
            }
        };

        lock (this)
        {
            var windows = cache.GetWindows(cache.FetchWindowDuration);

            foreach (var window in windows)
            {
                AggregateWindow(window);
            }
        }
    }

    private void PurgeExpiredItems()
    {
        //TODO: lastFetchTimestamp or MaxTimestamp or Now?
        //var upto = LastFetchTimestamp - SeriesDuration;
        var upto = MaxTimestamp - SeriesDuration;
        aggregatedCache.Purge(upto);
    }
}