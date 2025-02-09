using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq.Expressions;

[Pure]
public sealed class ExpressionEqualityComparer : IEqualityComparer<Expression>
{
    private ExpressionEqualityComparer()
    {
    }

    public static ExpressionEqualityComparer Instance { get; } = new();

    public bool Equals(Expression x, Expression y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        x = x.GetTheActualExpressionIfDecorated();
        y = y.GetTheActualExpressionIfDecorated();

        return object.Equals(x, y);
    }

    public int GetHashCode([DisallowNull] Expression obj)
    {
        if (obj is IExpressionDecorator decorator)
        {
            obj = decorator.Expression;
        }

        return obj.ToString().GetHashCode();
    }
}