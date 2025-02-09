using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq;

public static class QueryableExtensions
{
    private static readonly MethodInfo CacheTemplateMethod = typeof(QueryableExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m => m.Name == nameof(Cache) && m.IsGenericMethod);

    private static readonly MethodInfo CommentTemplateMethod = typeof(QueryableExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == nameof(Comment) && m.IsGenericMethod);

    private static readonly MethodInfo EnumerableToListTemplateMethod =
        typeof(Enumerable).GetMethod(nameof(Enumerable.ToList), BindingFlags.Public | BindingFlags.Static);

    private static readonly MethodInfo QueryableTakeTemplateMethod = typeof(Queryable).GetMethods()
        .First(m => m.Name == nameof(Queryable.Take) && m.GetParameters()[1].ParameterType == typeof(int));

    public static IQueryable BuildSelect(this IQueryable queryable, IEnumerable<string> propertyNames)
    {
        return queryable.BuildSelect(propertyNames, out var _);
    }

    public static IQueryable BuildSelect(this IQueryable queryable, IEnumerable<string> propertyNames,
        out Type newQueryableEntityType)
    {
        var queryableElementType = LinqHelper.GetLinqElementType(queryable);
        var typedParameterExpression = Expression.Parameter(queryableElementType);

        var properties = propertyNames.Select(queryableElementType.GetProperty).ToList();
        var dynamicProperties = properties.ConvertAll(x => new Tuple<string, Type>(x.Name, x.PropertyType));
        var anonymousType = AnonymousTypeBuilder.BuildAnonymousType(dynamicProperties);

        var propertyExpressionExpressions =
            properties.ConvertAll(x => Expression.Property(typedParameterExpression, x));

        var constructor = anonymousType.GetConstructorOfAnonymousType();
        var newExpression = Expression.New(constructor, propertyExpressionExpressions);

        Expression lambda = Expression.Lambda(newExpression, typedParameterExpression);
        var method =
            SharedReflectionInfo.QueryableSelectTemplateMethod.MakeGenericMethod(queryableElementType, anonymousType);

        var result = method.Invoke(null, [queryable, lambda]);
        newQueryableEntityType = anonymousType;

        return (IQueryable)result;
    }

    public static IQueryable BuildTake(this IQueryable queryable, int n)
    {
        var elementType = LinqHelper.GetLinqElementType(queryable);
        var method = QueryableTakeTemplateMethod.MakeGenericMethod(elementType);
        var result = method.Invoke(null, [queryable, n]);
        return (IQueryable)result;
    }

    public static IQueryable BuildWhere(this IQueryable queryable,
        IReadOnlyList<KeyValuePair<string, string>> propertyNamesAndValues, ExpressionType op)
    {
        var action = new WhereQueryableAction(propertyNamesAndValues, op);
        return action.Apply(queryable);
    }

    public static IQueryable<TSource> Cache<TSource>(this IQueryable<TSource> source, TimeSpan? duration)
    {
        return source.Provider.CreateQuery<TSource>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Cache, source, duration),
            [
                source.Expression,
                Expression.Constant(duration, typeof(TimeSpan?))
            ]));
    }

    public static IQueryable Cache(this IQueryable source, TimeSpan? duration)
    {
        var elementType = CoreReflectionHelper.GetEnumerableItemTypeIfEnumerable(source);

        var exp = Expression.Call(null, CacheTemplateMethod.MakeGenericMethod(elementType),
        [
            source.Expression,
            Expression.Constant(duration, typeof(TimeSpan?))
        ]);

        return source.Provider.CreateQuery(exp);
    }

    public static IQueryable<TSource> Comment<TSource>(this IQueryable<TSource> source, string comment)
    {
        return source.Provider.CreateQuery<TSource>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Comment, source, comment),
            [
                source.Expression,
                Expression.Constant(comment, typeof(string))
            ]));
    }

    public static IQueryable Comment(this IQueryable source, string comment)
    {
        var elementType = CoreReflectionHelper.GetEnumerableItemTypeIfEnumerable(source);

        var exp = Expression.Call(null, CommentTemplateMethod.MakeGenericMethod(elementType),
        [
            source.Expression,
            Expression.Constant(comment, typeof(string))
        ]);

        return source.Provider.CreateQuery(exp);
    }

    public static IList QueryableToList(this IQueryable queryable)
    {
        var elementType = CoreReflectionHelper.GetEnumerableItemTypeIfEnumerable(queryable);
        var list = EnumerableToListTemplateMethod.MakeGenericMethod(elementType).Invoke(null, [queryable]);
        return (IList)list;
    }

    public static IQueryable<TSource> RebasePreviousDateTimeRangeWindow<TSource>(this IQueryable<TSource> source)
    {
        return source.Provider.CreateQuery<TSource>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(RebasePreviousDateTimeRangeWindow, source), source.Expression));
    }
}