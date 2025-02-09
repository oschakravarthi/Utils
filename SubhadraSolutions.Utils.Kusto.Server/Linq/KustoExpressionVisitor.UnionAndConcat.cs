using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleUnionAndConcatMethods(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(KustoQueryableExtensions))
        {
            switch (node.Method.Name)
            {
                case nameof(KustoQueryableExtensions.Union):
                    {
                        var queryables = ((ConstantExpression)node.Arguments[1]).Value as IQueryable[];

                        var queryableExpressions = new Expression[queryables.Length];
                        for (var i = 0; i < queryables.Length; i++) queryableExpressions[i] = queryables[i].Expression;

                        var setQueryParts = new IQueryPart[queryableExpressions.Length + 1];
                        var setReferenceParts = new IQueryPart[setQueryParts.Length];

                        var part = Visit(node.Arguments[0]);
                        var partExp = (QueryPartExpression)part;

                        setQueryParts[0] = new SetQueryPart(partExp.QueryPart, false, queryContext);
                        // setReferenceParts[0] = new ReferenceQueryPart(setQueryParts[0]);
                        setReferenceParts[0] = setQueryParts[0];
                        var sb = new StringBuilder();
                        sb.Append("{0}");

                        for (var i = 0; i < queryableExpressions.Length; i++)
                        {
                            sb.Append(" | union ({").Append(i + 1).Append("})");
                            part = Visit(queryableExpressions[i]);
                            partExp = (QueryPartExpression)part;
                            setQueryParts[i + 1] = new SetQueryPart(partExp.QueryPart, false, queryContext);
                            // setReferenceParts[i + 1] = new ReferenceQueryPart(setQueryParts[i + 1]);
                            setReferenceParts[i + 1] = setQueryParts[i + 1];
                        }

                        var formatPart = new FormatQueryPart(sb.ToString(), setReferenceParts);
                        //var compositePart= new CompositeQueryPart(setQueryParts);
                        //return new QueryPartExpression(node, new CompositeQueryPart(compositePart, formatPart));
                        return new QueryPartExpression(node, formatPart);
                    }
                case nameof(KustoQueryableExtensions.FormatUnion):
                    {
                        var visitor = new KustoExpressionVisitor(new QueryContext());
                        var part = visitor.Visit(node.Arguments[0]);
                        var partExp = (QueryPartExpression)part;
                        var templateQueryPart = partExp.QueryPart;
                        var queryTemplate = templateQueryPart.GetQuery();

                        var argumentsArray = ((ConstantExpression)node.Arguments[1]).Value as object[][];
                        var setQueryParts = new IQueryPart[argumentsArray.Length];

                        for (var i = 0; i < argumentsArray.Length; i++)
                        {
                            var arguments = argumentsArray[i];
                            var parts = new IQueryPart[arguments.Length];

                            for (var j = 0; j < arguments.Length; j++)
                            {
                                var arg = arguments[j];
                                parts[j] = new LiteralQueryPart(arg?.ToString());
                            }

                            setQueryParts[i] = new SetQueryPart(new FormatQueryPart(queryTemplate, parts), false,
                                queryContext);
                            // this.queryContext.AddSetQueryPart(setQueryPart);
                        }

                        var setReferenceParts = new IQueryPart[setQueryParts.Length];
                        var sb = new StringBuilder();

                        for (var i = 0; i < setQueryParts.Length; i++)
                        {
                            if (i == 1)
                            // sb.Append(" | union (");
                            {
                                sb.Append(" | union ");
                            }

                            var setPart = setQueryParts[i];
                            sb.Append('{').Append(i).Append('}');

                            if (i > 0 && i < setQueryParts.Length - 1)
                            {
                                sb.Append(", ");
                            }

                            // setReferenceParts[i] = new ReferenceQueryPart(setPart);
                            setReferenceParts[i] = setPart;
                        }

                        // if (setQueryParts.Length > 1)
                        // {
                        //    sb.Append(")");
                        // }
                        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), setReferenceParts));
                        // return new QueryPartExpression(node, new CompositeQueryPart(new CompositeQueryPart(setQueryParts), new FormatQueryPart(sb.ToString(), setReferenceParts)));
                    }
            }

            return null;
        }

        if (node.Method.DeclaringType != typeof(Queryable))
        {
            return null;
        }

        switch (node.Method.Name)
        {
            case nameof(Queryable.Concat):
            case nameof(Queryable.Union):

                var part1 = Visit(node.Arguments[0]);
                var part1Exp = (QueryPartExpression)part1;

                var part2 = Visit(node.Arguments[1]);
                var part2Exp = (QueryPartExpression)part2;

                var setReferenceParts = new IQueryPart[2];
                // setReferenceParts[0] = new ReferenceQueryPart(part1Exp.QueryPart);
                setReferenceParts[0] = part1Exp.QueryPart;
                // setReferenceParts[1] = new ReferenceQueryPart(part2Exp.QueryPart);
                setReferenceParts[1] = part2Exp.QueryPart;
                return new QueryPartExpression(node,
                    new CompositeQueryPart(new CompositeQueryPart(part1Exp.QueryPart, part2Exp.QueryPart),
                        new FormatQueryPart("{0}\r\n| union ({1})", setReferenceParts)));
        }

        return null;
    }
}