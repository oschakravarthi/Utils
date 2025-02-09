using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private static readonly Dictionary<string, string> KustoMethodNamesLookup = new()
    {
        { nameof(StringHelper.Trim), "trim" },
        { nameof(StringHelper.TrimStart), "trim_start" },
        { nameof(StringHelper.TrimEnd), "trim_end" },
        { nameof(StringHelper.HasAllIgnoreCase), "has_all" },
        { nameof(StringHelper.HasAnyIgnoreCase), "has_any" }
    };

    private Expression HandleBuiltinStringMethod(MethodCallExpression node)
    {
        if (node.Method.ReturnType == typeof(string) && node.Method.Name == nameof(ToString))
        {
            return HandleStandardStringMethod(node);
        }

        if (node.Method.DeclaringType != typeof(string))
        {
            return null;
        }

        switch (node.Method.Name)
        {
            case nameof(string.IsNullOrEmpty):
                var part = Visit(node.Arguments[0]);
                var partExp = (QueryPartExpression)part;
                return new QueryPartExpression(node, new FormatQueryPart("isempty({0})", partExp.QueryPart));

            case nameof(string.StartsWith):
            case nameof(string.EndsWith):
            case nameof(string.Contains):
                var kustoMethodName = node.Method.Name.ToLower() + "_cs";
                var part1 = Visit(node.Object);
                var part2 = Visit(node.Arguments[0]);
                var part1Exp = (QueryPartExpression)part1;
                var part2Exp = (QueryPartExpression)part2;
                if (node.Arguments.Count > 1)
                {
                    var isIgnoreCase = IsIgnoreCase(node.Arguments[1]);
                    if (isIgnoreCase)
                    {
                        kustoMethodName = node.Method.Name.ToLower();
                    }
                }

                return new QueryPartExpression(node,
                    new FormatQueryPart("{0} " + kustoMethodName + " {1}", part1Exp.QueryPart, part2Exp.QueryPart));

            case nameof(string.Substring):
            case nameof(string.IndexOf):
            case nameof(string.ToUpper):
            case nameof(string.ToLower):
                return HandleStandardStringMethod(node);

            case nameof(string.Length):
                return HandleStandardStringMethod(node, "strlen");

            case nameof(string.Trim):
            case nameof(string.TrimStart):
            case nameof(string.TrimEnd):
                return HandleTrim(node);

            case nameof(string.Concat):
                return HandleStringConcat(node);

            case nameof(string.Replace):
                return HandleStandardStringMethod(node, "replace_string");

            case nameof(string.Equals):
                return HandleStringEquals(node);

            case nameof(string.Compare):
                return HandleStringCompare(node);
        }

        return null;
    }

    private static bool IsIgnoreCase(Expression expression)
    {
        var option = expression.GetConstant();
        if (option is bool b)
        {
            return b;
        }

        if (option is StringComparison sc)
        {
            return sc is StringComparison.OrdinalIgnoreCase or StringComparison.InvariantCultureIgnoreCase or StringComparison.CurrentCultureIgnoreCase;
        }

        return false;
    }

    private Expression HandleKustoStringMethod(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(StringHelper) &&
            node.Method.DeclaringType != typeof(KustoQueryableExtensions) &&
            node.Method.DeclaringType != typeof(QueryableExtensions))
        {
            return null;
        }

        var methodName = node.Method.Name;
        switch (methodName)
        {
            case nameof(StringHelper.Has):
            case nameof(StringHelper.HasPrefix):
            case nameof(StringHelper.HasSuffix):
                {
                    var part1 = Visit(node.Arguments[0]);
                    var ignoreCase = IsIgnoreCase(node.Arguments[1]);
                    var part2 = Visit(node.Arguments[2]);
                    var part1Exp = (QueryPartExpression)part1;
                    var part2Exp = (QueryPartExpression)part2;
                    var kustoMethodName = methodName.ToLower();

                    if (!ignoreCase)
                    {
                        kustoMethodName += "_cs";
                    }

                    return new QueryPartExpression(node,
                        new FormatQueryPart("{0} " + kustoMethodName + " {1}", part1Exp.QueryPart, part2Exp.QueryPart));
                }

            case nameof(StringHelper.Trim):
            case nameof(StringHelper.TrimEnd):
            case nameof(StringHelper.TrimStart):
                {
                    var part1 = Visit(node.Arguments[0]);
                    var part2 = Visit(node.Arguments[1]);
                    var part1Exp = (QueryPartExpression)part1;
                    var part2Exp = (QueryPartExpression)part2;

                    var kustoMethodName = KustoMethodNamesLookup[methodName];
                    return new QueryPartExpression(node,
                        new FormatQueryPart(kustoMethodName + "({0},{1})", part2Exp.QueryPart, part1Exp.QueryPart));
                }

            case nameof(StringHelper.MatchesRegex):
                {
                    var part1 = Visit(node.Arguments[0]);
                    var part2 = Visit(node.Arguments[1]);
                    var part1Exp = (QueryPartExpression)part1;
                    var part2Exp = (QueryPartExpression)part2;

                    return new QueryPartExpression(node,
                        new FormatQueryPart("{0} matches regex {1}", part1Exp.QueryPart, part2Exp.QueryPart));
                }
            case nameof(StringHelper.HasAllIgnoreCase):
            case nameof(StringHelper.HasAnyIgnoreCase):
                {
                    var part1 = Visit(node.Arguments[0]);
                    var part2 = Visit(node.Arguments[1]);
                    var part1Exp = (QueryPartExpression)part1;
                    var part2Exp = (QueryPartExpression)part2;

                    var kustoMethodName = KustoMethodNamesLookup[methodName];
                    return new QueryPartExpression(node,
                        new FormatQueryPart("{0} " + kustoMethodName + "({1})", part1Exp.QueryPart, part2Exp.QueryPart));
                }
            case nameof(StringHelper.In):
                {
                    var part1 = Visit(node.Arguments[0]);
                    var ignoreCase = IsIgnoreCase(node.Arguments[1]);
                    var part2 = Visit(node.Arguments[2]);
                    var part1Exp = (QueryPartExpression)part1;
                    var part2Exp = (QueryPartExpression)part2;
                    var kustoMethodName = methodName.ToLower();

                    if (ignoreCase)
                    {
                        kustoMethodName += "~";
                    }

                    return new QueryPartExpression(node,
                        new FormatQueryPart("{0} " + kustoMethodName + "({1})", part1Exp.QueryPart, part2Exp.QueryPart));
                }
        }

        return null;
    }

    private Expression HandleKustoStringParamsMethod(MethodCallExpression node, string methodName)
    {
        var part1 = Visit(node.Arguments[0]);
        var part2 = Visit(node.Arguments[1]);
        var part1Exp = (QueryPartExpression)part1;
        var part2Exp = (QueryPartExpression)part2;
        return new QueryPartExpression(node,
            new FormatQueryPart("{0} " + methodName + "({1})", part1Exp.QueryPart, part2Exp.QueryPart));
    }

    private Expression HandleStandardStringMethod(MethodCallExpression node, string methodName = null,
        bool argsInFirst = false)
    {
        var indexer = 0;
        var parts = new List<IQueryPart>();
        var sb = new StringBuilder();
        sb.AppendFormat("{0}(", methodName ?? node.Method.Name.ToLower());
        var argsCount = node.Arguments.Count;

        if (!argsInFirst)
        {
            sb.Append('{').Append(indexer).Append('}');
            var part = Visit(node.Object);
            var partExp = (QueryPartExpression)part;
            parts.Add(partExp.QueryPart);
            if (argsCount > 0)
            {
                sb.Append(", ");
            }

            indexer++;
        }

        for (var i = 0; i < node.Arguments.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append('{').Append(indexer).Append('}');

            var part = Visit(node.Arguments[i]);
            var partExp = (QueryPartExpression)part;
            parts.Add(partExp.QueryPart);
            indexer++;
        }

        if (argsInFirst)
        {
            if (argsCount > 0)
            {
                sb.Append(", ");
            }

            var part = Visit(node.Object);
            var partExp = (QueryPartExpression)part;
            parts.Add(partExp.QueryPart);
        }

        sb.Append(')');

        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), parts.ToArray()));
    }

    private Expression HandleStringCompare(MethodCallExpression node)
    {
        var part1 = Visit(node.Arguments[0]);
        var part2 = Visit(node.Arguments[1]);
        var part1Exp = (QueryPartExpression)part1;
        var part2Exp = (QueryPartExpression)part2;
        return new QueryPartExpression(node,
            new FormatQueryPart("strcmp({0}, {1})", part1Exp.QueryPart, part2Exp.QueryPart));
    }

    private Expression HandleStringConcat(MethodCallExpression node)
    {
        // var treeNode = this.queryContext.Tree.Find(node, ReferenceEqualityComparer.Instance);
        var arguments = node.Arguments;

        if (arguments.Count == 1 && arguments[0].NodeType == ExpressionType.NewArrayInit)
        {
            arguments = ((NewArrayExpression)arguments[0]).Expressions;
        }

        var args = new IQueryPart[arguments.Count];
        var sb = new StringBuilder();
        sb.Append("strcat(");

        for (var i = 0; i < arguments.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append('{').Append(i).Append('}');
            var part = Visit(arguments[i]);
            var partExp = (QueryPartExpression)part;
            args[i] = partExp.QueryPart;
        }

        sb.Append(')');
        return new QueryPartExpression(node, new FormatQueryPart(sb.ToString(), args));
    }

    private Expression HandleStringEquals(MethodCallExpression node)
    {
        var ignoreCase = false;

        Expression left;
        Expression right;
        if (node.Object == null)
        {
            left = node.Arguments[0];
            right = node.Arguments[1];

            if (node.Arguments.Count > 2)
            {
                ignoreCase = IsIgnoreCase(node.Arguments[2]);
            }
        }
        else
        {
            left = node.Object;
            right = node.Arguments[0];

            if (node.Arguments.Count > 1)
            {
                ignoreCase = IsIgnoreCase(node.Arguments[1]);
            }
        }

        var op = "==";
        if (ignoreCase)
        {
            op = "=~";
        }

        var part1 = Visit(left);
        var part2 = Visit(right);
        var part1Exp = (QueryPartExpression)part1;
        var part2Exp = (QueryPartExpression)part2;
        return new QueryPartExpression(node,
            new FormatQueryPart("{0}" + op + "{1}", part1Exp.QueryPart, part2Exp.QueryPart));
    }

    private Expression HandleStringMemberExpression(MemberExpression node)
    {
        if (node.Member.DeclaringType != typeof(string))
        {
            return null;
        }

        switch (node.Member.Name)
        {
            case "Length":
                var part = Visit(node.Expression);
                var partExp = (QueryPartExpression)part;
                return new QueryPartExpression(node, new FormatQueryPart("strlen({0})", partExp.QueryPart));
        }

        return null;
    }

    private Expression HandleStringMethod(MethodCallExpression node)
    {
        var returned = HandleBuiltinStringMethod(node);
        if (returned != null)
        {
            return returned;
        }

        return HandleKustoStringMethod(node);
    }

    // private void HandleStringConcat(MethodCallExpression node)
    // {
    //    //var treeNode = this.queryContext.Tree.Find(node, ReferenceEqualityComparer.Instance);
    //    IList<Expression> args = node.Arguments;
    //    if (args.Count == 1 && args[0].NodeType == ExpressionType.NewArrayInit)
    //    {
    //        args = ((NewArrayExpression)args[0]).Expressions;
    //    }
    //    sb.Append("concat(");
    //    for (int i = 0, n = args.Count; i < n; i++)
    //    {
    //        if (i > 0)
    //        {
    //            sb.Append(", ");
    //        }
    //        this.Visit(args[i]);
    //    }
    //    sb.Append(")");
    //    //return node;
    // }

    private Expression HandleTrim(MethodCallExpression node)
    {
        var methodName = KustoMethodNamesLookup[node.Method.Name];

        if (node.Arguments.Count == 0)
        {
            var part = Visit(node.Object);
            var partExp = (QueryPartExpression)part;
            return new QueryPartExpression(node, new FormatQueryPart(methodName + "(' ', {0})", partExp.QueryPart));
        }

        var newArrayExpression = node.Arguments[0] as NewArrayExpression;
        var expCount = newArrayExpression.Expressions.Count;

        if (expCount > 0)
        {
            var sb1 = new StringBuilder();

            for (var i = 0; i < expCount; i++)
            {
                sb1.Append(methodName).Append('(');
                sb1.Append(methodName).Append('{').Append(i).Append('}');
                Visit(newArrayExpression.Expressions[i]);
                sb1.Append(',');
            }

            var part = Visit(node.Object);
            var partExp = (QueryPartExpression)part;
            var sb2 = new StringBuilder();

            for (var i = 0; i < expCount; i++) sb2.Append(')');

            return new QueryPartExpression(node, new FormatQueryPart(sb1 + "{0}" + sb2, partExp.QueryPart));
        }

        return Visit(node.Object);
    }
}