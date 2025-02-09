using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public class PreviousDateTimeRangeExpressionVisitor : ExpressionVisitor
{
    private static readonly MethodInfo DateTime_op_AdditionDateTimespanMethod =
        typeof(DateTime).GetMethod("op_Addition", BindingFlags.Public | BindingFlags.Static);

    private static readonly MethodInfo DateTime_op_SubtractionDateDateMethod = typeof(DateTime)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m => m.Name == "op_Subtraction" && m.ReturnType == typeof(TimeSpan));

    private static readonly MethodInfo DateTime_op_SubtractionDateTimespanMethod = typeof(DateTime)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m => m.Name == "op_Subtraction" && m.ReturnType == typeof(DateTime));

    private static readonly MethodInfo QueryableSelectTemplateMethod = typeof(Queryable)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.Name == nameof(Queryable.Select) && m.IsGenericMethod && m.GetGenericArguments().Length == 2);

    private static readonly MethodInfo TimeSpanDurationMethod =
        typeof(TimeSpan).GetMethod(nameof(TimeSpan.Duration), BindingFlags.Public | BindingFlags.Instance);

    private bool foundTimerangedWhereClause;

    public PreviousDateTimeRangeExpressionVisitor(IQueryable queryable)
    {
        var newExpression = Visit(queryable.Expression);
        if (foundTimerangedWhereClause)
        {
            RewrittenQueryable = ((ICloneableQueryable)queryable).Clone(newExpression);
        }
    }

    public IQueryable RewrittenQueryable { get; }

    [return: NotNullIfNotNull("node")]
    public override Expression Visit(Expression node)
    {
        if (node is not MethodCallExpression callExpression)
        {
            return base.Visit(node);
        }

        if (!(callExpression.Method.DeclaringType == typeof(Queryable) &&
              callExpression.Method.Name == nameof(Queryable.Where)))
        {
            return base.Visit(node);
        }

        var lambda = callExpression.Arguments[1].GetLambda();
        if (lambda == null)
        {
            return base.Visit(node);
        }

        if (lambda.Body is not BinaryExpression binaryExpression)
        {
            return base.Visit(node);
        }

        if (!(binaryExpression.Left != null && binaryExpression.Right != null))
        {
            return base.Visit(node);
        }

        if (binaryExpression.Left is not BinaryExpression left || binaryExpression.Right is not BinaryExpression right)
        {
            return base.Visit(node);
        }

        if (!((left.NodeType is ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual &&
               right.NodeType is ExpressionType.LessThan or ExpressionType.LessThanOrEqual) ||
              (right.NodeType is ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual &&
               left.NodeType is ExpressionType.LessThan or ExpressionType.LessThanOrEqual)))
        {
            return base.Visit(node);
        }

        var entityType = lambda.Parameters[0].Type;
        var constructor = entityType.GetSuitableConstructor();
        if (constructor == null)
        {
            return base.Visit(node);
        }

        var member = left.Left.GetMember();
        if (member != right.Left.GetMember())
        {
            return base.Visit(node);
        }

        if (left.Left.Type != typeof(DateTime))
        {
            return base.Visit(node);
        }

        var date1 = left.Right;
        var date2 = right.Right;
        var temp = Expression.Call(DateTime_op_SubtractionDateDateMethod, date1, date2);
        var durationExpression = Expression.Call(temp, TimeSpanDurationMethod);

        var leftsNewRight = Expression.Call(DateTime_op_SubtractionDateTimespanMethod, left.Right, durationExpression);
        var rightsNewRight =
            Expression.Call(DateTime_op_SubtractionDateTimespanMethod, right.Right, durationExpression);

        left = Expression.MakeBinary(left.NodeType, left.Left, leftsNewRight);
        right = Expression.MakeBinary(right.NodeType, right.Left, rightsNewRight);

        binaryExpression = Expression.MakeBinary(binaryExpression.NodeType, left, right);
        lambda = Expression.Lambda(binaryExpression, lambda.Parameters.ToArray());
        callExpression = Expression.Call(callExpression.Method, callExpression.Arguments[0], lambda);

        var operand = BuildLambdaForSelect(entityType, constructor, member, durationExpression);

        if (operand == null)
        {
            return node;
        }

        var selectMethod = QueryableSelectTemplateMethod.MakeGenericMethod(entityType, entityType);

        callExpression = Expression.Call(selectMethod, callExpression, operand);
        foundTimerangedWhereClause = true;
        return callExpression;
    }

    private static LambdaExpression BuildLambdaForSelect(Type entityType, ConstructorInfo constructor,
        MemberInfo member, MethodCallExpression durationExpression)
    {
        var parameterExpression = Expression.Parameter(entityType);

        var expressions = new List<KeyValuePair<MemberInfo, Expression>>();
        var properties = entityType.GetProperties().Where(p => p.GetIndexParameters().Length == 0);
        var found = false;
        foreach (var property in properties)
        {
            Expression expression = Expression.Property(parameterExpression, property);
            if (property.Name == member.Name)
            {
                found = true;
                expression = Expression.Call(DateTime_op_AdditionDateTimespanMethod, expression, durationExpression);
            }

            expressions.Add(new KeyValuePair<MemberInfo, Expression>(property, expression));
        }

        if (!found)
        {
            return null;
        }

        Expression finalExpression = null;

        if (constructor.GetParameters().Length > 0)
        {
            finalExpression = Expression.New(constructor, expressions.Select(kvp => kvp.Value).ToArray());
        }
        else
        {
            var bindings = expressions.Select(kvp => Expression.Bind(kvp.Key, kvp.Value)).ToArray();
            finalExpression = Expression.MemberInit(Expression.New(constructor), bindings);
        }

        return Expression.Lambda(finalExpression, parameterExpression);
    }
}