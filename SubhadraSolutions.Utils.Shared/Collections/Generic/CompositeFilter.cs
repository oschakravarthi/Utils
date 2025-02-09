using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class CompositeFilter<T> : IFilter<T>
{
    public List<IFilter<T>> InnerFilters { get; set; } = [];

    public bool ShouldFilter(T obj)
    {
        foreach (var filter in InnerFilters)
        {
            if (filter.ShouldFilter(obj))
            {
                return true;
            }
        }

        return false;
    }
}