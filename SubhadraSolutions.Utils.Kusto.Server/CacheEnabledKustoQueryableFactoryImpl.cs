using Kusto.Data.Common;
using Microsoft.Extensions.Caching.Memory;
using SubhadraSolutions.Utils.Kusto.Server.Contracts;
using SubhadraSolutions.Utils.Kusto.Server.Linq;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Kusto.Server;

public class CacheEnabledKustoQueryableFactoryImpl(IMemoryCache cache, TimeOnly absoluteExpiry) : IKustoQueryableFactory
{
    public IQueryable<T> CreateKustoQueryable<T>(ICslQueryProvider cslQueryProvider, string query)
    {
        IQueryable<T> queryable = new KustoQueryable<T>(cslQueryProvider, query);
        return CacheAndReturn(queryable);
    }

    public IQueryable<T> CreateKustoQueryable<T>(ICslQueryProvider cslQueryProvider, IQueryPart queryPart)
    {
        IQueryable<T> queryable = new KustoQueryable<T>(cslQueryProvider, queryPart);
        return CacheAndReturn(queryable);
    }

    private IQueryable<T> CacheAndReturn<T>(IQueryable<T> queryable)
    {
        if (cache != null)
        //Comment below line to disable caching.
        {
            queryable = new CachedQueryableDecorator<T>(queryable, KustoCacheKeyBuilder.Instance, cache,
                absoluteExpiry);
        }

        return queryable;
    }
}