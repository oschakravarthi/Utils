using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.SlidingWindows.Contracts;

public interface ISlidingWindowCache<T> : IUnique // : IDataProvider<T>
{
    TimeSpan FetchWindowDuration { get; }

    DateTime LastFetchTimestamp { get; }

    DateTime MaxTimestamp { get; }

    TimeSpan SeriesDuration { get; }

    event EventHandler<DataFetchEventArgs<T>> OnDataFetch;

    IQueryable<T> GetAllItems();

    IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration);

    IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration, TimeSpan jumpDuration);

    IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration, uint windowsToSkip, uint maxWindowsToFetch);
}