using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.SlidingWindows;

public class InMemoryAggregateSlidingWindowCache<TFrom, TTo> : AbstractInMemoryAggregateSlidingWindowCache<TFrom, TTo>
    where TTo : ITimeRanged
{
    public InMemoryAggregateSlidingWindowCache(ISlidingWindowCache<TFrom> cache,
        Func<Window<TFrom>, IReadOnlyList<TTo>> aggregator, TimeSpan seriesDuration)
        : base(cache, aggregator, seriesDuration)
    {
        Name = typeof(TTo) + " InMemory SlidingWindowCache";
    }
}