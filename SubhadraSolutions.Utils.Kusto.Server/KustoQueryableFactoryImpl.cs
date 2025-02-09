using Kusto.Data.Common;
using SubhadraSolutions.Utils.Kusto.Server.Contracts;
using SubhadraSolutions.Utils.Kusto.Server.Linq;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using System.Linq;

namespace SubhadraSolutions.Utils.Kusto.Server;

public sealed class KustoQueryableFactoryImpl : IKustoQueryableFactory
{
    private KustoQueryableFactoryImpl()
    {
    }

    public static KustoQueryableFactoryImpl Instance { get; } = new();

    public IQueryable<T> CreateKustoQueryable<T>(ICslQueryProvider cslQueryProvider, string query)
    {
        return new KustoQueryable<T>(cslQueryProvider, query);
    }

    public IQueryable<T> CreateKustoQueryable<T>(ICslQueryProvider cslQueryProvider, IQueryPart queryPart)
    {
        return new KustoQueryable<T>(cslQueryProvider, queryPart);
    }
}