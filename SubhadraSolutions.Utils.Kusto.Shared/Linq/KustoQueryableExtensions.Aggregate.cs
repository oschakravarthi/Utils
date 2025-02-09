using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public static partial class KustoQueryableExtensions
{
    public static IQueryable<TSource> ArgMax<TSource, T>(this IQueryable<TSource> source,
        Expression<Func<TSource, T>> selector)
    {
        return source.Provider.CreateQuery<TSource>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(ArgMax, source, selector),
            [
                source.Expression,
                selector
            ]));
    }

    public static IQueryable<TSource> ArgMin<TSource, T>(this IQueryable<TSource> source,
        Expression<Func<TSource, T>> selector)
    {
        return source.Provider.CreateQuery<TSource>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(ArgMin, source, selector),
            [
                source.Expression,
                selector
            ]));
    }

    public static PackedList<TResult> MakeBag<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector)
    {
        return source.Provider.Execute<PackedList<TResult>>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(MakeBag, source, selector),
            [
                source.Expression,
                selector
            ]));
    }

    public static PackedList<TResult> MakeSet<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector)
    {
        return source.Provider.Execute<PackedList<TResult>>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(MakeSet, source, selector),
            [
                source.Expression,
                selector
            ]));
    }

    public static PackedList<TResult> MakeSet<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector, SortingOrder sortingOrder)
    {
        return source.Provider.Execute<PackedList<TResult>>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(MakeSet, source, selector, sortingOrder),
            [
                source.Expression,
                selector,
                Expression.Constant(sortingOrder)
            ]));
    }

    public static TResult Pack<TSource, TResult>(this TSource source, Expression<Func<TSource, TResult>> selector)
    {
        throw new NotImplementedException();
    }

    //public static List<TSource> BagUnpack<TSource>(this PackedList<TSource> source)
    //{
    //    throw new NotImplementedException();
    //}
    public static TResult PackAnonymous<TSource, TResult>(this TSource source,
        Expression<Func<TSource, TResult>> selector)
    {
        throw new NotImplementedException();
    }

    public static List<TSource> Unpack<TSource>(this PackedList<TSource> source)
    {
        throw new NotImplementedException();
    }
}