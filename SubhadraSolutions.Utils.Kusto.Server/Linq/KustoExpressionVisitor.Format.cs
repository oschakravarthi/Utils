using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleFormatMethod(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(KustoQueryableExtensions))
        {
            switch (node.Method.Name)
            {
                case nameof(KustoQueryableExtensions.Format):
                    // var visitor = new KustoExpressionVisitor(new QueryContext(null));
                    // var part = visitor.Visit(node.Arguments[0]);
                    var part = Visit(node.Arguments[0]);
                    var partExp = (QueryPartExpression)part;
                    var templateQueryPart = partExp.QueryPart;

                    var arguments = (object[])((ConstantExpression)node.Arguments[1]).Value;
                    var parts = new IQueryPart[arguments.Length];
                    for (var i = 0; i < arguments.Length; i++)
                    {
                        var arg = arguments[i];
                        parts[i] = new LiteralQueryPart(arg?.ToString());
                    }

                    var setQueryPart = new SetQueryPart(new FormatQueryPart(templateQueryPart, parts), false,
                        queryContext);
                    // this.queryContext.AddSetQueryPart(setQueryPart);
                    return new QueryPartExpression(node, setQueryPart);
                    //return new QueryPartExpression(node, new FormatQueryPart(templateQueryPart, parts));
            }
        }

        return null;
    }
}