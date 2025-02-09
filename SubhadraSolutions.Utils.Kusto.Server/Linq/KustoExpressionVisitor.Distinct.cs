using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleDistinct(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Queryable) &&
            node.Method.DeclaringType != typeof(KustoQueryableExtensions))
        {
            return null;
        }

        if (node.Method.Name != nameof(Queryable.Distinct) &&
            node.Method.Name != nameof(Queryable.DistinctBy))
        {
            return null;
        }

        var part1 = Visit(node.Arguments[0]);
        var part1Exp = (QueryPartExpression)part1;

        switch (node.Method.Name)
        {
            case nameof(Queryable.Distinct):
                {
                    const string template = "{0}\r\n| distinct *";
                    return new QueryPartExpression(node, new FormatQueryPart(template, part1Exp.QueryPart));
                }
            case nameof(Queryable.DistinctBy):
                {
                    const string template = "{0}\r\n| distinct {1}";
                    var part2 = Visit(node.Arguments[1]);
                    var part2Exp = (QueryPartExpression)part2;
                    return new QueryPartExpression(node,
                        new FormatQueryPart(template, part1Exp.QueryPart, part2Exp.QueryPart));
                }
        }

        return null;
    }
}