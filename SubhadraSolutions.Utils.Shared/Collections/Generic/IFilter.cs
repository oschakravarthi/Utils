namespace SubhadraSolutions.Utils.Collections.Generic;

public interface IFilter<in T>
{
    bool ShouldFilter(T obj);
}