using Kusto.Data.Common;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using System.Linq;

namespace SubhadraSolutions.Utils.Kusto.Server.Contracts;

public interface IKustoQueryableFactory
{
    IQueryable<T> CreateKustoQueryable<T>(ICslQueryProvider cslQueryProvider, string query);

    IQueryable<T> CreateKustoQueryable<T>(ICslQueryProvider cslQueryProvider, IQueryPart queryPart);
}