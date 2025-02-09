using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SubhadraSolutions.Utils.SlidingWindows;

public abstract class AbstractSlidingWindowDataCache<T> : AbstractInitializableAndDisposable, IUnique
    where T : ITimeRanged
{
    private readonly IDataProvider<T> dataProvider;
    private readonly TimeSpan? historyReplayDuration;
    private readonly IntervalTask intervalTask;

    protected AbstractSlidingWindowDataCache(IDataProvider<T> dataProvider, TimeSpan seriesDuration,
        TimeSpan fetchWindowDuration, TimeSpan? historyReplayDuration)
    {
        this.dataProvider = dataProvider;
        SeriesDuration = seriesDuration;
        FetchWindowDuration = fetchWindowDuration;
        if (historyReplayDuration != null)
        {
            if (historyReplayDuration.Value < seriesDuration)
            {
                historyReplayDuration = seriesDuration;
            }
        }

        this.historyReplayDuration = historyReplayDuration;
        Name = typeof(T).Name;
        LastFetchTimestamp = GlobalSettings.Instance.DateTimeNow -
                             (this.historyReplayDuration == null ? SeriesDuration : historyReplayDuration.Value);
        MaxTimestamp = LastFetchTimestamp;

        intervalTask = new IntervalTask((long)FetchWindowDuration.TotalMilliseconds, true);
    }

    public TimeSpan FetchWindowDuration { get; }
    public DateTime LastFetchTimestamp { get; private set; }
    public DateTime MaxTimestamp { get; private set; }
    public TimeSpan SeriesDuration { get; }
    public string Name { get; set; }

    public event EventHandler<DataFetchEventArgs<T>> OnDataFetch;

    public IQueryable<T> GetAllItems()
    {
        var items = GetAllItemsCore();
        return items;
    }

    public virtual IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration)
    {
        var allItems = GetAllItems();
        return WindowingHelper.SplitIntoWindows(allItems, windowDuration);
    }

    public virtual IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration, TimeSpan jumpDuration)
    {
        var allItems = GetAllItems();
        return WindowingHelper.SplitIntoWindows(allItems, windowDuration, jumpDuration);
    }

    public virtual IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration, uint windowsToSkip,
        uint maxWindowsToFetch)
    {
        var allItems = GetAllItems();
        return WindowingHelper.SplitIntoWindows(allItems, windowDuration, windowsToSkip, maxWindowsToFetch);
    }

    protected virtual void AddItems(IReadOnlyList<T> items)
    {
    }

    protected abstract IQueryable<T> GetAllItemsCore();

    protected override void InitializeProtected()
    {
        var now = GlobalSettings.Instance.DateTimeNow;
        var from = now - (historyReplayDuration ?? SeriesDuration);
        ThreadPool.QueueUserWorkItem(o =>
        {
            FetchNewItemsCore(from, now);
            intervalTask.AddAction(LoadPastData, null);
        });

        //IntervalMultiTask.DefaultInstance.AddAction(LoadPastData, (long)FetchWindowDuration.TotalMilliseconds, null);
    }

    protected abstract void PurgeExpiredItems(DateTime upto);

    private void FetchNewItemsCore(DateTime from, DateTime upto)
    {
        var originalFrom = from;
        while (from < upto)
        {
            var next = from + FetchWindowDuration;
            var items = dataProvider.GetData(from, next);
            LastFetchTimestamp = next;
            if (items.Count > 0)
            {
                UpdateMax(items[0]);
                AddItems(items);
            }

            PurgeExpiredItems();

            FireOnFetch(from, next, items);
            from = next;
        }
    }

    private void FireOnFetch(DateTime from, DateTime upto, IReadOnlyList<T> items)
    {
        var onDataFetch = OnDataFetch;
        if (onDataFetch != null)
        {
            var window = new Window<T>(from, upto, items);
            var args = new DataFetchEventArgs<T>(window, MaxTimestamp);
            onDataFetch(this, args);
        }
    }

    private void LoadPastData(object tag)
    {
        LoadPastData();
    }

    private void LoadPastData()
    {
        var from = MaxTimestamp;
        var now = GlobalSettings.Instance.DateTimeNow;
        FetchNewItemsCore(from, now);
    }

    private void PurgeExpiredItems()
    {
        //TODO: lastFetchTimestamp or MaxTimestamp or Now?
        //var upto = LastFetchTimestamp - SeriesDuration;
        var upto = MaxTimestamp - SeriesDuration;
        PurgeExpiredItems(upto);
    }

    private void UpdateMax(T item)
    {
        var timestamp = item.GetUptoTimestamp();
        if (MaxTimestamp < timestamp)
        {
            MaxTimestamp = timestamp;
        }
    }
}