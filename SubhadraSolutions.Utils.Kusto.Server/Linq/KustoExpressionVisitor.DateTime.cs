using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private static readonly HashSet<string> DateParts = new(StringComparer.CurrentCultureIgnoreCase)
    {
        "year", "quarter", "month", "week_of_year", "day", "dayofyear", "hour", "minute", "second", "millisecond",
        "microsecond", "nanosecond"
    };

    private Expression HandleDateAddMethod(MethodCallExpression node, string word)
    {
        var part1 = Visit(node.Arguments[0]);
        var part1Exp = (QueryPartExpression)part1;
        var part2 = Visit(node.Object);
        var part2Exp = (QueryPartExpression)part2;
        string multiplier = null;
        switch (word)
        {
            case "day":
                multiplier = "*24*60*60*1000";
                break;

            case "hour":
                multiplier = "*60*60*1000";
                break;

            case "minute":
                multiplier = "*60*1000";
                break;

            case "second":
                multiplier = "*1000";
                break;
        }

        if (multiplier != null)
        {
            word = "millisecond";
        }

        return new QueryPartExpression(node,
            new FormatQueryPart($"datetime_add('{word}'" + ",tolong({0}{1}),{2})", part1Exp.QueryPart,
                new LiteralQueryPart(multiplier), part2Exp.QueryPart));
    }

    private Expression HandleDateTimeMemberExpression(MemberExpression node)
    {
        if (node.Member.DeclaringType != typeof(DateTime) &&
            node.Member.DeclaringType != typeof(KustoQueryableExtensions))
        {
            return null;
        }

        switch (node.Member.Name)
        {
            //case nameof(DateTime.AddDays):
            //    sb.Append("datetime_add('day',");
            //    Visit(node.Arguments[0]);
            //    sb.Append(",");
            //    Visit(node.Object);
            //    sb.Append(")");
            //    return node;
            case nameof(DateTime.Now):
                return new QueryPartExpression(node, new LiteralQueryPart("now()"));

            case nameof(DateTime.UtcNow):
                // TODO:
                return new QueryPartExpression(node, new LiteralQueryPart("now()"));

            case nameof(DateTime.Date):

            case nameof(DateTime.Day):
            case nameof(DateTime.DayOfWeek):
            case nameof(DateTime.DayOfYear):
            case nameof(DateTime.Hour):
            case nameof(DateTime.Year):
            case nameof(DateTime.Month):
            case nameof(DateTime.Minute):
            case nameof(DateTime.Second):
            case nameof(DateTime.Millisecond):
                break;

            default:
                return null;
        }

        var part = Visit(node.Expression);
        var partExp = (QueryPartExpression)part;

        switch (node.Member.Name)
        {
            //case nameof(DateTime.AddDays):
            //    sb.Append("datetime_add('day',");
            //    Visit(node.Arguments[0]);
            //    sb.Append(",");
            //    Visit(node.Object);
            //    sb.Append(")");
            //    return node;
            case nameof(DateTime.Date):
                return new QueryPartExpression(node, new FormatQueryPart("startofday({0})", partExp.QueryPart));

            case nameof(DateTime.Day):
                return new QueryPartExpression(node, new FormatQueryPart("dayofmonth({0})", partExp.QueryPart));

            case nameof(DateTime.DayOfWeek):
                return new QueryPartExpression(node, new FormatQueryPart("dayofweek({0})", partExp.QueryPart));

            case nameof(DateTime.DayOfYear):
                return new QueryPartExpression(node, new FormatQueryPart("dayofyear({0})", partExp.QueryPart));

            case nameof(DateTime.Hour):
                return new QueryPartExpression(node, new FormatQueryPart("hourofday({0})", partExp.QueryPart));

            case nameof(DateTime.Year):
            case nameof(DateTime.Month):
            case nameof(DateTime.Minute):
            case nameof(DateTime.Second):
            case nameof(DateTime.Millisecond):
                return new QueryPartExpression(node,
                    new FormatQueryPart("datetime_part(\"" + node.Member.Name.ToLower() + "\", {0})",
                        partExp.QueryPart));
        }

        return null;
    }

    private Expression HandleDateTimeMethod(MethodCallExpression node)
    {
        var method = node.Method;
        if (method.DeclaringType != typeof(DateTime) && method.DeclaringType != typeof(KustoQueryableExtensions) &&
            method.DeclaringType != typeof(DateTimeHelper))
        {
            return null;
        }

        switch (method.Name)
        {
            case nameof(DateTime.AddYears):
                return HandleDateAddMethod(node, "year");

            case nameof(KustoQueryableExtensions.AddQuarters):
                return HandleDateAddMethod(node, "quarter");

            case nameof(DateTime.AddMonths):
                return HandleDateAddMethod(node, "month");

            case nameof(KustoQueryableExtensions.AddWeeks):
                return HandleDateAddMethod(node, "week");

            case nameof(DateTime.AddDays):
                return HandleDateAddMethod(node, "day");

            case nameof(DateTime.AddHours):
                return HandleDateAddMethod(node, "hour");

            case nameof(DateTime.AddMinutes):
                return HandleDateAddMethod(node, "minute");

            case nameof(DateTime.AddSeconds):
                return HandleDateAddMethod(node, "second");

            case nameof(KustoQueryableExtensions.Quarter):
            case nameof(KustoQueryableExtensions.Microsecond):
            case nameof(KustoQueryableExtensions.Nanosecond):
                {
                    var part = Visit(node.Arguments[0]);
                    var partExp = (QueryPartExpression)part;
                    var template = $"datetime_part(\"{node.Method.Name.ToLower()}\", " + "{0})";
                    return new QueryPartExpression(node, new FormatQueryPart(template, partExp.QueryPart));
                }
            case nameof(KustoQueryableExtensions.WeekOfYear):
                {
                    var part = Visit(node.Arguments[0]);
                    var partExp = (QueryPartExpression)part;
                    const string template = "datetime_part(\"week_of_year\", {0})";
                    return new QueryPartExpression(node, new FormatQueryPart(template, partExp.QueryPart));
                }
            case nameof(DateTimeHelper.StartOfWeek):
            case nameof(DateTimeHelper.StartOfMonth):
            case nameof(DateTimeHelper.EndOfMonth):
            case nameof(DateTimeHelper.EndOfWeek):
                {
                    var part = Visit(node.Arguments[0]);
                    var partExp = (QueryPartExpression)part;
                    var template = method.Name.ToLower() + "({0})";
                    return new QueryPartExpression(node, new FormatQueryPart(template, partExp.QueryPart));
                }

            case nameof(DateTime.ToString):
                return HandleDateToStringMethod(node);
        }

        return null;
    }

    private Expression HandleDateToStringMethod(MethodCallExpression node)
    {
        var isFullDate = false;

        var datePart = Visit(node.Object);
        var datePartExp = (QueryPartExpression)datePart;

        IQueryPart argPart;

        if (node.Arguments.Count > 0)
        {
            var arg = Visit(node.Arguments[0]);
            var argPartExp = (QueryPartExpression)arg;
            argPart = argPartExp.QueryPart;
        }
        else
        {
            argPart = new LiteralQueryPart(GlobalSettings.Instance.DateAndTimeSerializationFormat);
            isFullDate = true;
        }

        var argValue = argPart.GetQuery();

        if (!isFullDate && DateParts.Contains(argValue.Trim('"')))
        {
            return new QueryPartExpression(node,
                new FormatQueryPart("datetime_part({0}, {1})", argPart, datePartExp.QueryPart));
        }

        return new QueryPartExpression(node,
            new FormatQueryPart("format_datetime({0}, {1})", datePartExp.QueryPart, argPart));
    }
}