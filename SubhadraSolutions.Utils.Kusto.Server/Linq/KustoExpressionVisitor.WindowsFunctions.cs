using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleWindowFunctions(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case nameof(KustoQueryableExtensions.RowCumSum):
                var part = Visit(node.Arguments[0]);
                var partExp = (QueryPartExpression)part;
                return new QueryPartExpression(node, new FormatQueryPart("row_cumsum({0})", partExp.QueryPart));
        }

        return null;
    }
}