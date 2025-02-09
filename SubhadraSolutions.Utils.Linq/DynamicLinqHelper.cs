using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq;

public static class DynamicLinqHelper
{
    public static Delegate CompileDynamicLinqExpression(string expression,
        params Tuple<string, Type>[] parameterNameAndTypes)
    {
        var parameters = new ParameterExpression[parameterNameAndTypes.Length];
        for (var i = 0; i < parameterNameAndTypes.Length; i++)
        {
            var tuple = parameterNameAndTypes[i];
            parameters[i] = Expression.Parameter(tuple.Item2, tuple.Item1);
        }

        var e = DynamicExpressionParser.ParseLambda(parameters, null, expression);
        var compiledExpression = e.Compile();
        return compiledExpression;
    }
}