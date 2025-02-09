using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Linq.Expressions;
using System;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal partial class KustoExpressionVisitor
{
    private static string GetOperator(BinaryExpression binaryExpression)
    {
        switch (binaryExpression.NodeType)
        {
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                return " and ";

            case ExpressionType.Or:
            case ExpressionType.OrElse:
                return " or ";

            default:
                var op = ExpressionHelper.GetOperator(binaryExpression.NodeType);
                if (op == null)
                {
                    throw new NotSupportedException(
                        $"The binary operator '{binaryExpression.NodeType}' is not supported");
                }

                // return " " + op + " ";
                return op;
        }
    }

    private Expression HandleBinary(BinaryExpression node)
    {
        if (node.Method != null)
        {
            var callMethod = Expression.Call(node.Method, node.Left, node.Right);
            return Visit(callMethod);
        }

        if (node.Left == null || node.Right == null)
        {
            return HandleBinaryWhenOneSideisNull(node);
        }

        var partLeft = (QueryPartExpression)Visit(node.Left);
        var partRight = (QueryPartExpression)Visit(node.Right);
        if (partLeft.QueryPart == NullQueryPart.Instance || partRight.QueryPart == NullQueryPart.Instance)
        {
            return HandleBinaryWhenOneSideisNull(node);
        }

        return HandleBinaryStandard(node);
    }

    private Expression HandleBinaryStandard(BinaryExpression node)
    {
        var op = GetOperator(node);
        var part1 = Visit(node.Left);
        var part2 = Visit(node.Right);
        var part1Exp = (QueryPartExpression)part1;
        var part2Exp = (QueryPartExpression)part2;
        return new QueryPartExpression(node,
            new FormatQueryPart("{0}" + op + "{1}", part1Exp.QueryPart, part2Exp.QueryPart));
    }

    private Expression HandleBinaryWhenOneSideisNull(BinaryExpression node)
    {
        if (node.Left == null && node.Right == null)
        {
            return new QueryPartExpression(node, EmptyQueryPart.Instance);
        }

        string method = null;

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                method = "isnull";
                break;

            case ExpressionType.NotEqual:
                method = "isnotnull";
                break;
        }

        var left = node.Left ?? node.Right;
        var part = Visit(left);
        var partExp = (QueryPartExpression)part;
        var queryPart = partExp.QueryPart;
        //A dirty fix for a condition like .Select(x=>x==null)
        if (queryPart == EmptyQueryPart.Instance || queryPart == NullQueryPart.Instance)
        {
            queryPart = new LiteralQueryPart("\"\"");
        }

        return new QueryPartExpression(node, new FormatQueryPart(method + "({0})", queryPart));
    }

    private Expression HandleUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Not)
        {
            var part = Visit(node.Operand);
            var partExp = (QueryPartExpression)part;
            // return new QueryPartExpression(partExp.Expression, new FormatQueryPart("not({0})", partExp.QueryPart));
            return new QueryPartExpression(node, new FormatQueryPart("not({0})", partExp.QueryPart));
        }

        return Visit(node.Operand);
    }
}