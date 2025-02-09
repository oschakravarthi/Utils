using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Linq.Expressions;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq;

public static class LinqHelper
{
    public static AttributesLookup BuildAttributesLookup(IQueryable queryable)
    {
        var seeds = ExploreToSeed(queryable);
        return new AttributesLookup(seeds);
    }

    public static IEnumerable<MethodInfo> ExploreCallChain(Expression expression)
    {
        var enumerable = ExploreCallChainExpressions(expression);
        foreach (var e in enumerable)
        {
            yield return e.Method;
        }
    }

    public static IEnumerable<MethodCallExpression> ExploreCallChainExpressions(Expression expression)
    {
        while (expression != null)
        {
            expression = expression.GetTheActualExpressionIfDecorated();
            var callExpression = expression as MethodCallExpression;
            expression = null;
            if (callExpression != null)
            {
                yield return callExpression;
                if (callExpression.Arguments.Count > 0)
                {
                    expression = callExpression.Arguments[0].GetTheActualExpressionIfDecorated();
                }
            }
        }
    }

    public static IEnumerable<Type> ExploreToSeed(IQueryable queryable)
    {
        var expression = queryable.Expression;
        expression = expression.GetTheActualExpressionIfDecorated();
        if (expression != null)
        {
            while (expression != null)
            {
                var type = expression.Type;

                if (type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type))
                {
                    yield return type.GetGenericArguments()[0];
                }

                var callExpression = expression as MethodCallExpression;
                expression = null;
                if (callExpression?.Arguments.Count > 0)
                {
                    expression = callExpression.Arguments[0].GetTheActualExpressionIfDecorated();
                }
            }
        }
    }

    public static Type GetLinqElementType(IQueryable queryable)
    {
        return GetLinqElementType(queryable.ElementType);
    }

    public static Type GetLinqElementType(Type seqType)
    {
        var ienum = seqType.FindIEnumerable();

        if (ienum == null)
        {
            return seqType;
        }

        return ienum.GetGenericArguments()[0];
    }

    public static Type GetSeedElementType(IQueryable queryable, bool breakOnNonAnonymousElementType)
    {
        Type result = null;
        foreach (var type in ExploreToSeed(queryable))
        {
            result = type;

            if (breakOnNonAnonymousElementType && !result.IsAnonymousType())
            {
                return result;
            }
        }

        return result;
    }
}