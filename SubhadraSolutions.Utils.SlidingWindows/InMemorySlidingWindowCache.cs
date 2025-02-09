using SubhadraSolutions.Utils.Collections.Generic;
using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SubhadraSolutions.Utils.SlidingWindows;

public class InMemorySlidingWindowCache<T> : AbstractSlidingWindowDataCache<T>, ISlidingWindowCache<T>
    where T : class, ITimeRanged, new()
{
    private readonly TimeRangedLinkedList<T> cache = [];
    private readonly ReaderWriterLockSlim lockSlim = new();

    public InMemorySlidingWindowCache(IDataProvider<T> dataProvider, TimeSpan seriesDuration,
        TimeSpan fetchWindowDuration, TimeSpan? historyReplayDuration)
        : base(dataProvider, seriesDuration, fetchWindowDuration, historyReplayDuration)
    {
        Name = typeof(T) + " InMemory SlidingWindowCache";
    }

    protected override void AddItems(IReadOnlyList<T> items)
    {
        Interner<T>.Intern(items);
        cache.Add(items);
    }

    protected override void Dispose(bool disposing)
    {
        lockSlim.Dispose();
    }

    protected override IQueryable<T> GetAllItemsCore()
    {
        return cache.AsQueryable();
    }

    protected override void PurgeExpiredItems(DateTime upto)
    {
        var previous = cache.Count;
        cache.Purge(upto);
        var now = cache.Count;
        //if(previous>0&& previous==now)
        //{
        //    //Console.WriteLine("{0}\tPreviousCacheSize: {1}, Now: {2}, Diff: {3}", this.Name, previous, this.cache.Count, previous - this.cache.Count);

        //    Console.WriteLine("Min: {0}, Max: {1}, Upto: {2}, MaxTimestamp: {3}", GlobalSettings.DateTimeToString(this.cache.Oldest.GetFromTimestamp()), GlobalSettings.DateTimeToString(this.cache.Latest.GetUptoTimestamp()), GlobalSettings.DateTimeToString(upto), GlobalSettings.DateTimeToString(MaxTimestamp));
        //}
    }

    //public IReadOnlyList<T> GetData(DateTime from, DateTime upto)
    //{
    //    return this.cache.GetWindow(from, upto).ToList().AsReadOnly();
    //}
}