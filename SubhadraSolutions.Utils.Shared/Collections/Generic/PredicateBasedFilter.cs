using SubhadraSolutions.Utils.Validation;
using System;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class PredicateBasedFilter<T> : IFilter<T>
{
    private readonly Predicate<T> predicate;

    public PredicateBasedFilter(Predicate<T> predicate)
    {
        Guard.ArgumentShouldNotBeNull(predicate, nameof(predicate));
        this.predicate = predicate;
    }

    public bool ShouldFilter(T obj)
    {
        return predicate(obj);
    }
}