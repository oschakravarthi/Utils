using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    public Expression HandleOperatorOverloadedMethod(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case "op_GreaterThan":
                return HandleOperatorOverloadedMethod(node, ">");

            case "op_GreaterThanOrEqual":
                return HandleOperatorOverloadedMethod(node, ">=");

            case "op_LessThan":
                return HandleOperatorOverloadedMethod(node, "<");

            case "op_LessThanOrEqual":
                return HandleOperatorOverloadedMethod(node, "<=");

            case "op_Equality":
                return HandleOperatorOverloadedMethod(node, "==");

            case "op_Inequality":
                return HandleOperatorOverloadedMethod(node, "!=");

            case "op_Subtraction":
                return HandleOperatorOverloadedMethod(node, "-");

            case "op_Addition":
                return HandleOperatorOverloadedMethod(node, "+");
        }

        return null;
    }

    private Expression HandleOperatorOverloadedMethod(MethodCallExpression node, string op)
    {
        var part1 = Visit(node.Arguments[0]);
        var part2 = Visit(node.Arguments[1]);
        var part1Exp = (QueryPartExpression)part1;
        var part2Exp = (QueryPartExpression)part2;

        if (part2Exp.QueryPart == NullQueryPart.Instance)
        {
            if (op == "==")
            {
                return new QueryPartExpression(node, new FormatQueryPart("isnull({0})", part1Exp.QueryPart));
            }

            if (op == "!=")
            {
                return new QueryPartExpression(node, new FormatQueryPart("isnotnull({0})", part1Exp.QueryPart));
            }
        }

        return new QueryPartExpression(node,
            new FormatQueryPart("{0}" + op + "{1}", part1Exp.QueryPart, part2Exp.QueryPart));
    }
}