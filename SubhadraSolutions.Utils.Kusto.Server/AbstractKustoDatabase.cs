using Kusto.Data.Common;
using SubhadraSolutions.Utils.Kusto.Server.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Kusto.Server;

public abstract class AbstractKustoDatabase
{
    private readonly ICslQueryProvider cslQueryProvider;
    private readonly Dictionary<Type, string> queriesLookup = [];

    protected AbstractKustoDatabase(ICslQueryProvider cslQueryProvider)
    {
        this.cslQueryProvider = cslQueryProvider;
    }

    protected void Register<T>(string query)
    {
        Register(typeof(T), query);
    }

    protected void Register(Type type, string query)
    {
        queriesLookup.Add(type, query);
    }

    protected IQueryable<T> GetQueryable<T>()
    {
        if (queriesLookup.TryGetValue(typeof(T), out var query))
        {
            return new KustoQueryable<T>(cslQueryProvider, query);
        }

        return null;
    }
}