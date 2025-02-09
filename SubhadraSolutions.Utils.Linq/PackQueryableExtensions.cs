using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Linq.Expressions;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq;

public static class PackQueryableExtensions
{
    public static MemberAssignment Average<TTarget>(this IQueryable<int> source,
        Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Int32_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<int?> source,
        Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableInt32_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<long> source,
        Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Int64_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<long?> source,
        Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableInt64_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<float> source,
        Expression<Func<TTarget, float>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Single_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<float?> source,
        Expression<Func<TTarget, float?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableSingle_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<double> source,
        Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Double_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<double?> source,
        Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableDouble_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<decimal> source,
        Expression<Func<TTarget, decimal>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Decimal_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TTarget>(this IQueryable<decimal?> source,
        Expression<Func<TTarget, decimal?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableDecimal_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, int>> selector, Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Int32_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, int?>> selector, Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableInt32_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, float>> selector, Expression<Func<TTarget, float>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Single_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, float?>> selector, Expression<Func<TTarget, float?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableSingle_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, long>> selector, Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Int64_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, long?>> selector, Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableInt64_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, double>> selector, Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Double_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, double?>> selector, Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableDouble_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, decimal>> selector, Expression<Func<TTarget, decimal>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_Decimal_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Average<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, decimal?>> selector, Expression<Func<TTarget, decimal?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Average_NullableDecimal_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Build<TTarget, TValue>(MethodCallExpression expression,
        Expression<Func<TTarget, TValue>> mapper)
    {
        var memberExpression = mapper.Body.GetMemberExpression();
        var member = memberExpression.Member;
        var property = member as PropertyInfo;
        return Expression.Bind(property, expression);
    }

    [DynamicallyInvoked]
    public static IQueryable<TSource> GetPackedScalarsLocal<TSource>(this IQueryable<TSource> source,
        params MemberAssignment[] assignments)
    {
        var packedExpression = Expression.Constant(assignments);
        var methodInfo = typeof(PackQueryableExtensions).GetMethod(nameof(GetPackedScalarsLocal),
            BindingFlags.Public | BindingFlags.Static);
        methodInfo = methodInfo.MakeGenericMethod(typeof(TSource));
        var callExp = Expression.Call(null, methodInfo, source.Expression, packedExpression);

        return (IQueryable<TSource>)source.Provider.CreateQuery(callExp);
    }

    public static MemberAssignment Max<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TTarget, TSource>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Max_TSource_1(typeof(TSource)), source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Max<TSource, TResult, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector, Expression<Func<TTarget, TResult>> mapper)
    {
        var callExp = Expression.Call(null,
            CachedReflectionInfo.Max_TSource_TResult_2(typeof(TSource), typeof(TResult)), source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Min<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TTarget, TSource>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Min_TSource_1(typeof(TSource)), source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Min<TSource, TResult, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector, Expression<Func<TTarget, TResult>> mapper)
    {
        var callExp = Expression.Call(null,
            CachedReflectionInfo.Min_TSource_TResult_2(typeof(TSource), typeof(TResult)), source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<int> source, Expression<Func<TTarget, int>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Int32_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<int?> source, Expression<Func<TTarget, int?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableInt32_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<long> source, Expression<Func<TTarget, long>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Int64_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<long?> source, Expression<Func<TTarget, long?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableInt64_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<float> source, Expression<Func<TTarget, float>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Single_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<float?> source,
        Expression<Func<TTarget, float?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableSingle_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<double> source,
        Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Double_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<double?> source,
        Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableDouble_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<decimal> source,
        Expression<Func<TTarget, decimal>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Decimal_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TTarget>(this IQueryable<decimal?> source,
        Expression<Func<TTarget, decimal?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableDecimal_1, source.Expression);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, int>> selector, Expression<Func<TTarget, int>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Int32_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, int?>> selector, Expression<Func<TTarget, int?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableInt32_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, long>> selector, Expression<Func<TTarget, long>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Int64_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, long?>> selector, Expression<Func<TTarget, long?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableInt64_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, float>> selector, Expression<Func<TTarget, float>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Single_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, float?>> selector, Expression<Func<TTarget, float?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableSingle_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, double>> selector, Expression<Func<TTarget, double>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Double_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, double?>> selector, Expression<Func<TTarget, double?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableDouble_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, decimal>> selector, Expression<Func<TTarget, decimal>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_Decimal_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }

    public static MemberAssignment Sum<TSource, TTarget>(this IQueryable<TSource> source,
        Expression<Func<TSource, decimal?>> selector, Expression<Func<TTarget, decimal?>> mapper)
    {
        var callExp = Expression.Call(null, CachedReflectionInfo.Sum_NullableDecimal_TSource_2(typeof(TSource)),
            source.Expression, selector);
        return Build(callExp, mapper);
    }
}