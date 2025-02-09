using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using SubhadraSolutions.Utils.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private static string GetAggregateMethodName(MethodCallExpression node)
    {
        var methodName = node.Method.Name;
        switch (methodName)
        {
            case nameof(Queryable.Average):
                return "avg";

            case nameof(KustoQueryableExtensions.DistinctCount):
                return "dcount";

            case nameof(KustoQueryableExtensions.MakeSet):
                return "make_set";

            case nameof(KustoQueryableExtensions.MakeBag):
                return "make_bag";

            case nameof(KustoQueryableExtensions.ArgMin):
                return "arg_min";

            //case nameof(KustoQueryableExtensions.PackAnonymous):
            //    return "pack";

            //case nameof(KustoQueryableExtensions.BagUnpack):
            //    return "bag_unpack";

            case nameof(KustoQueryableExtensions.EvaluateBagUnpack):
                return "bag_unpack";

            case nameof(KustoQueryableExtensions.ArgMax):
                return "arg_max";

            default:
                return node.Method.Name.ToLower();
        }
    }

    private static bool IsExpressionResultsIntoQueryable(Expression expression)
    {
        var isQueryable = false;
        var exp = expression;

        if (exp is QueryPartExpression expression1)
        {
            exp = expression1.Expression;
        }
        else
        {
            if (exp is MethodCallExpression callExp)
            {
                if (typeof(IQueryable).IsAssignableFrom(callExp.Type))
                {
                    isQueryable = true;
                }
            }
        }

        if (!isQueryable && exp is ConstantExpression constExp)
        {
            isQueryable = constExp.Value is IQueryable;
        }

        return isQueryable;
    }

    private Expression HandleAggregateMethod(MethodCallExpression node)
    {
        if (!(node.Method.DeclaringType == typeof(Queryable) ||
              node.Method.DeclaringType == typeof(KustoQueryableExtensions)))
        {
            return null;
        }

        var isPreviousAQueryable = IsExpressionResultsIntoQueryable(node.Arguments[0]);

        switch (node.Method.Name)
        {
            case nameof(Queryable.Sum):
            case nameof(Queryable.Min):
            case nameof(Queryable.Max):
            case nameof(Queryable.Average):
            case nameof(KustoQueryableExtensions.MakeSet):
            case nameof(KustoQueryableExtensions.MakeBag):
            case nameof(KustoQueryableExtensions.Unpack):
            case nameof(KustoQueryableExtensions.DistinctCount):
            case nameof(KustoQueryableExtensions.ArgMax):
            case nameof(KustoQueryableExtensions.ArgMin):
                {
                    var sb = new StringBuilder();
                    sb.Append("{0}");
                    var parts = new List<IQueryPart>();
                    var part = Visit(node.Arguments[0]);

                    var partExp = (QueryPartExpression)part;

                    if (node.Method.Name == nameof(KustoQueryableExtensions.Unpack))
                    {
                        return new QueryPartExpression(node, new FormatQueryPart("tostring({0})", partExp.QueryPart));
                    }

                    parts.Add(partExp.QueryPart);

                    if (isPreviousAQueryable)
                    {
                        sb.Append(" | summarize ");
                    }

                    var methodName = GetAggregateMethodName(node);
                    var isSortedMakeSet = false;
                    if (node.Method.Name == nameof(KustoQueryableExtensions.MakeSet) && node.Arguments.Count == 3)
                    {
                        var constExp = node.Arguments[2] as ConstantExpression;
                        var sortOrder = (SortingOrder)constExp.Value;
                        var kustoMethod = sortOrder == SortingOrder.Ascending ? "array_sort_asc" : "array_sort_desc";
                        sb.Append(kustoMethod).Append('(');
                        isSortedMakeSet = true;
                    }

                    sb.Append(methodName).Append('(');
                    const int indexer = 1;

                    if (node.Arguments.Count > 1)
                    {
                        sb.Append('{').Append(indexer).Append('}');
                        part = Visit(node.Arguments[1]);
                        partExp = (QueryPartExpression)part;
                        parts.Add(partExp.QueryPart);
                    }

                    if (node.Method.Name is nameof(KustoQueryableExtensions.ArgMax) or nameof(KustoQueryableExtensions.ArgMin))
                    {
                        sb.Append(", *");
                    }

                    sb.Append(')');
                    if (isSortedMakeSet)
                    {
                        sb.Append(')');
                    }

                    return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), parts.ToArray()));
                }
            case nameof(Queryable.Count):
                {
                    var sb = new StringBuilder();
                    sb.Append("{0}");
                    var parts = new List<IQueryPart>();
                    var part = Visit(node.Arguments[0]);
                    var partExp = (QueryPartExpression)part;
                    parts.Add(partExp.QueryPart);

                    if (isPreviousAQueryable)
                    {
                        if (node.Arguments.Count > 1)
                        {
                            sb.Append(" | where ");

                            for (var i = 1; i < node.Arguments.Count; i++)
                            {
                                sb.Append('{').Append(i).Append('}');
                                part = Visit(node.Arguments[i]);
                                partExp = (QueryPartExpression)part;
                                parts.Add(partExp.QueryPart);
                            }
                        }

                        sb.Append(" | count ");
                        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), parts.ToArray()));
                    }

                    return new QueryPartExpression(node, new LiteralQueryPart(" count()"));
                }
            case nameof(KustoQueryableExtensions.Pack):
            case nameof(KustoQueryableExtensions.PackAnonymous):
                {
                    var selectorExp = (QueryPartExpression)Visit(node.Arguments[1]);
                    var splits = selectorExp.QueryPart.GetQuery().Split(',');
                    var sb = new StringBuilder();
                    if (isPreviousAQueryable)
                    {
                        sb.Append(" | summarize ");
                    }

                    var methodName = "pack";
                    sb.Append(methodName).Append('(');
                    for (var i = 0; i < splits.Length; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(',');
                        }

                        var trimmed = splits[i].Trim();
                        if (node.Method.Name == nameof(KustoQueryableExtensions.PackAnonymous))
                        {
                            sb.Append($"{trimmed}");
                        }
                        else
                        {
                            sb.Append(string.Format("\"{0}\", {0}", trimmed));
                        }
                    }

                    sb.Append(')');

                    var part = (QueryPartExpression)Visit(node.Arguments[0]);
                    return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), part.QueryPart));
                }
            case nameof(KustoQueryableExtensions.EvaluateBagUnpack):
                {
                    var member = node.Arguments[1].GetMember();
                    var template = "{0}| evaluate bag_unpack({1})";
                    var part = (QueryPartExpression)Visit(node.Arguments[0]);
                    return new QueryPartExpression(node,
                        new FormatQueryPart(template, part.QueryPart, new LiteralQueryPart(member.Name)));
                }
        }

        return null;
    }
}