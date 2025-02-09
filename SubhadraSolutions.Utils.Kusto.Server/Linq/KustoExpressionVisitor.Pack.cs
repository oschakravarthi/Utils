using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandlePackMethod(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(PackQueryableExtensions))
        {
            return null;
        }

        if (node.Method.Name != nameof(PackQueryableExtensions.GetPackedScalarsLocal))
        {
            return null;
        }

        var assignments = ((ConstantExpression)node.Arguments[1]).Value as MemberAssignment[];
        var parts = new SetQueryPart[assignments.Length];
        var temp = "temp" + queryContext.Sequencer.Next;
        var sb = new StringBuilder("range " + temp + " from 1 to 1 step 1 | extend ");

        for (var i = 0; i < assignments.Length; i++)
        {
            var assignment = assignments[i];

            IQueryPart queryPart;

            if (!visitedExpressions.TryGetValue(assignment.Expression, out var part))
            {
                part = Visit(assignment.Expression);
                var partExp = (QueryPartExpression)part;

                if (partExp.QueryPart is SetQueryPart setQueryPart)
                {
                    queryPart = setQueryPart;
                }
                else
                {
                    queryPart = new SetQueryPart(new FormatQueryPart("{0}", partExp.QueryPart), false, queryContext);
                }
            }
            else
            {
                var partExp = (QueryPartExpression)part;
                queryPart = partExp.QueryPart;
            }

            var property = assignment.Member;
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append(property.Name).Append(" = {").Append(i).Append('}');

            parts[i] = new SetQueryPart(new FormatQueryPart("toscalar({0})", queryPart), false, queryContext);
        }

        var finalQueryPart = new SetQueryPart(new FormatQueryPart(sb.ToString(), parts), false, queryContext);
        //string query = KustoQueryProvider.BuildQuery(finalQueryPart);

        return new QueryPartExpression(node, finalQueryPart);
    }
}