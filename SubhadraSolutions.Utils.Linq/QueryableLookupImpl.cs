using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public class QueryableLookupImpl : IQueryableLookup
{
    private readonly Dictionary<Type, Func<IQueryable>> lookup = [];

    public IQueryable<T> GetQueryable<T>()
    {
        var type = typeof(T);
        var queryable = GetQueryable(type);
        return (IQueryable<T>)queryable;
    }

    public IQueryable GetQueryable(Type entityType)
    {
        if (!lookup.TryGetValue(entityType, out var queryableFactory))
        {
            return null;
        }

        return queryableFactory();
    }

    public void RegisterQueryableFactory<T>(Func<IQueryable<T>> queryableFactory)
    {
        var type = typeof(T);
        lookup[type] = queryableFactory;
    }
}