using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Linq.Expressions;
using System;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;

public class QueryPartExpression(Expression expression, IQueryPart queryPart) : Expression, IExpressionDecorator
{
    public override sealed bool CanReduce => Expression.CanReduce;
    public override ExpressionType NodeType => Expression.NodeType;
    public IQueryPart QueryPart { get; } = queryPart;
    public override sealed Type Type => Expression.Type;
    public Expression Expression { get; } = expression;

    public override Expression Reduce()
    {
        var reduced = Expression.Reduce();
        return new QueryPartExpression(reduced, QueryPart);
    }

    public override string ToString()
    {
        return QueryPart.GetQuery();
    }

    public static Expression GetActualExpression(Expression expression)
    {
        if (expression is QueryPartExpression qexp)
        {
            return GetActualExpression(qexp.Expression);
        }

        return expression;
    }
}