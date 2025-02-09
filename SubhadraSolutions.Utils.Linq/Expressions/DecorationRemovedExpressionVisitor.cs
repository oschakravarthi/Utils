using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public class DecorationRemovedExpressionVisitor : ExpressionVisitor
{
    [return: NotNullIfNotNull("node")]
    public override Expression Visit(Expression node)
    {
        node = node.GetTheActualExpressionIfDecorated();
        return base.Visit(node);
    }
}