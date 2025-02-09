using SubhadraSolutions.Utils.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

public sealed class KustoCacheKeyBuilder : ICacheKeyBuilder
{
    private KustoCacheKeyBuilder()
    {
    }

    public static KustoCacheKeyBuilder Instance { get; } = new();

    public string BuildKey(Expression expression)
    {
        var queryContext = new QueryContext();
        var query = KustoQueryProvider.BuildQuery(expression, queryContext);
        if (queryContext.ClientRequestProperties.Parameters == null ||
            queryContext.ClientRequestProperties.Parameters.Count == 0)
        {
            return query;
        }

        var parameters = queryContext.ClientRequestProperties.Parameters;
        var keys = parameters.Keys.ToList();
        keys.Sort();
        var sb = new StringBuilder();
        for (var i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            sb.Append($"{key}:{parameters[key]},");
        }

        return query + sb;
    }
}