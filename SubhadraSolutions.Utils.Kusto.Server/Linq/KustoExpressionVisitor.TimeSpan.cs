using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private Expression HandleTimeSpanDurationMethod(MethodCallExpression node)
    {
        var part1 = Visit(node.Object);
        var part1Exp = (QueryPartExpression)part1;

        return new QueryPartExpression(node, new FormatQueryPart("abs({0})", part1Exp.QueryPart));
    }

    private Expression HandleTimeSpanMemberExpression(MemberExpression node)
    {
        if (node.Member.DeclaringType != typeof(TimeSpan) &&
            node.Member.DeclaringType != typeof(KustoQueryableExtensions))
        {
            return null;
        }

        var part = Visit(node.Expression);
        var partExp = (QueryPartExpression)part;

        switch (node.Member.Name)
        {
            case nameof(TimeSpan.Days):
            case nameof(TimeSpan.Hours):
            case nameof(TimeSpan.Milliseconds):
            case nameof(TimeSpan.Minutes):
            case nameof(TimeSpan.Seconds):
            case nameof(TimeSpan.Ticks):
                return new QueryPartExpression(node, new FormatQueryPart("tolong((({0})/1tick))", partExp.QueryPart));

            case nameof(TimeSpan.TotalDays):
                return new QueryPartExpression(node, new FormatQueryPart("(({0})/1d)", partExp.QueryPart));

            case nameof(TimeSpan.TotalHours):
                return new QueryPartExpression(node, new FormatQueryPart("(({0})/1h)", partExp.QueryPart));

            case nameof(TimeSpan.TotalMilliseconds):
                return new QueryPartExpression(node, new FormatQueryPart("(({0})/1ms)", partExp.QueryPart));

            case nameof(TimeSpan.TotalMinutes):
                return new QueryPartExpression(node, new FormatQueryPart("(({0})/1m)", partExp.QueryPart));

            case nameof(TimeSpan.TotalSeconds):
                return new QueryPartExpression(node, new FormatQueryPart("(({0})/1s)", partExp.QueryPart));

            default:
                return null;
        }
    }

    private Expression HandleTimeSpanMethod(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(TimeSpan))
        {
            return null;
        }

        switch (node.Method.Name)
        {
            case nameof(TimeSpan.Duration):
                return HandleTimeSpanDurationMethod(node);
        }

        return null;
    }
}