using SubhadraSolutions.Utils.Collections.Generic;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public static class ExpressionTreeExporter
{
    public static Tree<Expression> ExportAsTree(Expression expression)
    {
        var rootNode = Write(expression);
        return new Tree<Expression>(rootNode);
    }

    private static TreeNode<Expression> Write(Expression expression)
    {
        var actualExpression = expression.GetTheActualExpressionIfDecorated();

        var node = new TreeNode<Expression>(actualExpression);

        var type = actualExpression.GetType();
        var properties = type.GetProperties();
        var children = new List<TreeNode<Expression>>();

        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(actualExpression);

            if (propertyValue is Expression exp)
            {
                var child = Write(exp);
                children.Add(child);
            }
            else
            {
                if (propertyValue is IEnumerable<Expression> enumerable)
                {
                    foreach (var e in enumerable)
                    {
                        if (e != null)
                        {
                            var child = Write(e);
                            children.Add(child);
                        }
                    }
                }
            }
        }

        if (children.Count > 0)
        {
            foreach (var child in children)
            {
                node.Nodes.Add(child);
            }
        }

        return node;
    }
}