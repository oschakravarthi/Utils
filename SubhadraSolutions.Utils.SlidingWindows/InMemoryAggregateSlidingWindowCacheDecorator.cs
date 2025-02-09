using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.SlidingWindows;

public class InMemoryAggregateSlidingWindowCacheDecorator<T> : AbstractInMemoryAggregateSlidingWindowCache<T, T>
    where T : ITimeRanged
{
    public InMemoryAggregateSlidingWindowCacheDecorator(ISlidingWindowCache<T> inner,
        Func<Window<T>, IReadOnlyList<T>> aggregator, TimeSpan seriesDuration) :
        base(inner, aggregator, seriesDuration)
    {
        Name = typeof(T) + " Aggregated InMemory SlidingWindowCache";
    }

    public InMemoryAggregateSlidingWindowCacheDecorator<T> Decorate(TimeSpan seriesDuration)
    {
        return new InMemoryAggregateSlidingWindowCacheDecorator<T>(this, aggregator, seriesDuration);
    }
}