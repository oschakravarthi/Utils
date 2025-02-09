using SubhadraSolutions.Utils.Kusto.Shared.Linq;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

public static class PackKustoQueryableExtensions
{
    public static MemberAssignment DistinctCount<TSource, TF, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, TF>> selector, Expression<Func<TTarget, int>> mapper)
    {
        var callExp = Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(KustoQueryableExtensions.DistinctCount, source, selector),
            [
                source.Expression,
                selector
            ]);

        return PackQueryableExtensions.Build(callExp, mapper);
    }
}