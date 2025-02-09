using SubhadraSolutions.Utils.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public static partial class KustoQueryableExtensions
{
    public static IQueryable<T> FormatUnion<T>(this IQueryable<T> source, params object[][] argumentsArray)
    {
        return source.Provider.CreateQuery<T>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(FormatUnion, source, argumentsArray),
            [
                source.Expression,
                Expression.Constant(argumentsArray)
            ]));
    }

    public static IQueryable<T> Union<T>(this IQueryable<T> source, params IQueryable<T>[] queryables)
    {
        return source.Provider.CreateQuery<T>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Union, source, queryables),
            [
                source.Expression,
                Expression.Constant(queryables)
            ]));
    }
}