using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq.Expressions;

public static class ExpressionHelper
{
    public static object GetConstant(this Expression expression)
    {
        var constantExpression = expression.GetTypedExpression<ConstantExpression>();
        if (constantExpression != null)
        {
            return constantExpression.Value;
        }

        return null;
    }

    public static LambdaExpression GetLambda(this Expression expression)
    {
        return expression.GetTypedExpression<LambdaExpression>();
    }

    public static MemberInfo GetMember(this Expression expression)
    {
        var memberExpression = expression.GetMemberExpression();
        return memberExpression == null ? null : memberExpression.Member;
    }

    public static MemberExpression GetMemberExpression(this Expression expression)
    {
        if (expression is not MemberExpression memberExpression)
        {
            var lambdaExpression = expression.GetLambda();

            if (lambdaExpression == null)
            {
                if (expression is UnaryExpression unary)
                {
                    return unary.Operand.GetMemberExpression();
                }

                return null;
            }

            return lambdaExpression.Body.GetMemberExpression();
        }

        return memberExpression;
    }

    public static string GetOperator(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Not:
                return "!";

            case ExpressionType.Add:
            case ExpressionType.AddChecked:
                return "+";

            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
                return "-";

            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
                return "*";

            case ExpressionType.Divide:
                return "/";

            case ExpressionType.Modulo:
                return "%";

            case ExpressionType.And:
                return "&";

            case ExpressionType.AndAlso:
                return "&&";

            case ExpressionType.Or:
                return "|";

            case ExpressionType.OrElse:
                return "||";

            case ExpressionType.LessThan:
                return "<";

            case ExpressionType.LessThanOrEqual:
                return "<=";

            case ExpressionType.GreaterThan:
                return ">";

            case ExpressionType.GreaterThanOrEqual:
                return ">=";

            case ExpressionType.Equal:
                return "==";

            case ExpressionType.NotEqual:
                return "!=";

            case ExpressionType.Coalesce:
                return "??";

            case ExpressionType.RightShift:
                return ">>";

            case ExpressionType.LeftShift:
                return "<<";

            case ExpressionType.ExclusiveOr:
                return "^";

            default:
                return null;
        }
    }

    public static string[] GetParameterNames(this LambdaExpression lambdaExpression)
    {
        var count = lambdaExpression.Parameters.Count;
        var result = new string[count];

        for (var i = 0; i < count; i++) result[i] = lambdaExpression.Parameters[i].Name;

        return result;
    }

    public static Expression GetTheActualExpressionIfDecorated(this Expression expression)
    {
        if (expression == null)
        {
            return null;
        }

        while (true)
        {
            if (expression is not IExpressionDecorator decorated)
            {
                return expression;
            }

            expression = decorated.Expression;
        }
    }

    public static T GetTypedExpression<T>(this Expression expression) where T : Expression
    {
        var typedExpression = expression as T;

        if (typedExpression == null && expression is UnaryExpression unary)
        {
            return unary.Operand.GetTypedExpression<T>();
        }

        return typedExpression;
    }

    public static bool IfLambdaAndIsOneOfTheParametersTheBody(this Expression expression)
    {
        var lambdaExpression = expression.GetLambda();

        if (lambdaExpression == null)
        {
            return false;
        }

        return lambdaExpression.IsOneOfTheParametersTheBody();
    }

    public static Type IfLambdaGetReturnType(this Expression expression)
    {
        var lambdaExpression = expression.GetLambda();
        if (lambdaExpression == null)
        {
            return null;
        }

        return lambdaExpression.ReturnType;
    }

    public static bool IsOneOfTheParametersTheBody(this LambdaExpression lambdaExpression)
    {
        for (var i = 0; i < lambdaExpression.Parameters.Count; i++)
        {
            var parameter = lambdaExpression.Parameters[i];

            if (lambdaExpression.Body == parameter)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsPrimitiveParameterExpression(this ParameterExpression expression)
    {
        if (expression == null)
        {
            return false;
        }

        return expression.GetType().Name.StartsWith("PrimitiveParameterExpression");
    }
}