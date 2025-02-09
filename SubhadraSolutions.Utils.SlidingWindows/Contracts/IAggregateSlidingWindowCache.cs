using SubhadraSolutions.Utils.Contracts;

namespace SubhadraSolutions.Utils.SlidingWindows.Contracts;

public interface IAggregateSlidingWindowCache<T> : IDataProvider<T>, ISlidingWindowCache<T> where T : ITimeRanged
{
}