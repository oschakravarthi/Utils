using SubhadraSolutions.Utils.Linq.Expressions;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SubhadraSolutions.Utils.Linq;

public class WhereQueryableAction(IReadOnlyList<KeyValuePair<string, string>> propertyNamesAndValues, ExpressionType op)
    : IQueryableAction, IEquatable<WhereQueryableAction>,
        IComparable<WhereQueryableAction>
{
    private static readonly MethodInfo WhereMethodInfo;

    private readonly List<KeyValuePair<string, string>> propertyNamesAndValues = new(propertyNamesAndValues);

    private string toStringValue;

    static WhereQueryableAction()
    {
        var methods = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "Where").ToList();

        for (var i = 0; i < methods.Count; i++)
        {
            var method = methods[i];
            var parameters = method.GetParameters();
            var expressionType = parameters[1].ParameterType.GetGenericArguments()[0];

            if (expressionType.GetGenericArguments().Length == 2)
            {
                WhereMethodInfo = method;
                return;
            }
        }
    }

    public ExpressionType Operator { get; } = op;
    public IReadOnlyList<KeyValuePair<string, string>> PropertyNamesAndValues => propertyNamesAndValues.AsReadOnly();

    public int CompareTo(WhereQueryableAction other)
    {
        if (other == null)
        {
            return 1;
        }

        var result = propertyNamesAndValues.Count.CompareTo(other.propertyNamesAndValues.Count);
        if (result != 0)
        {
            return result;
        }

        for (var i = 0; i < propertyNamesAndValues.Count; i++)
        {
            var thisKVP = propertyNamesAndValues[i];
            var otherKVP = other.propertyNamesAndValues[i];
            result = thisKVP.Key.CompareTo(otherKVP.Key);

            if (result != 0)
            {
                return result;
            }

            result = thisKVP.Value.CompareTo(otherKVP.Value);
            if (result != 0)
            {
                return result;
            }
        }

        return 0;
    }

    public bool Equals(WhereQueryableAction other)
    {
        return CompareTo(other) == 0;
    }

    public IQueryable Apply(IQueryable input)
    {
        var elementType = LinqHelper.GetLinqElementType(input);
        var lambda = BuildLambdaToApply(elementType);

        var concreteMethod = WhereMethodInfo.MakeGenericMethod(elementType);
        var result = concreteMethod.Invoke(null, [input, lambda]);
        return (IQueryable)result;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as WhereQueryableAction);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override string ToString()
    {
        return toStringValue ??= BuildToStringRepresentation();
    }

    public IEnumerable<string> ToStrings()
    {
        var opString = ExpressionHelper.GetOperator(Operator);

        for (var i = 0; i < propertyNamesAndValues.Count; i++)
        {
            var kvp = propertyNamesAndValues[i];
            yield return $"({kvp.Key} {opString} {kvp.Value})";
        }
    }

    private Expression BuildLambdaToApply(Type elementType)
    {
        var expressions =
            PropertiesToStringValuesHelper.BuildValueExpressionFromStringValues(elementType, propertyNamesAndValues);
        var parameterExpression = Expression.Parameter(elementType, "e");

        Expression topException = null;

        for (var i = 0; i < propertyNamesAndValues.Count; i++)
        {
            var propertyName = elementType.GetProperty(propertyNamesAndValues[i].Key);
            Expression expression = Expression.Property(parameterExpression, propertyName.GetGetMethod());
            expression = Expression.MakeBinary(Operator, expression, expressions[i]);

            if (topException == null)
            {
                topException = expression;
            }
            else
            {
                topException = Expression.MakeBinary(ExpressionType.AndAlso, topException, expression);
            }
        }

        return Expression.Lambda(typeof(Func<,>).MakeGenericType(elementType, typeof(bool)), topException,
            parameterExpression);
    }

    private string BuildToStringRepresentation()
    {
        var sb = new StringBuilder();
        var i = 0;

        foreach (var s in ToStrings())
        {
            if (i > 0)
            {
                sb.Append(" AND ");
            }

            sb.Append(s);
            i++;
        }

        return sb.ToString();
    }
}