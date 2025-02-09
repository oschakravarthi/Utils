using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.SlidingWindows;

public static class Extensions
{
    public static InMemoryAggregateSlidingWindowCacheDecorator<T> DecorateWithInMemoryAggregateSlidingWindowCache<T>(
        this ISlidingWindowCache<T> cache, Func<Window<T>, IReadOnlyList<T>> aggregator, TimeSpan seriesDuration)
        where T : ITimeRanged
    {
        return new InMemoryAggregateSlidingWindowCacheDecorator<T>(cache, aggregator, seriesDuration);
    }
}