using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public static partial class KustoQueryableExtensions
{
    private static readonly MethodInfo MultiOrderByTemplateMethod = typeof(KustoQueryableExtensions).GetMethods()
        .First(m => m.Name == nameof(MultiOrderBy) && m.IsGenericMethod);

    private static readonly MethodInfo ProjectAwayTemplateMethod =
        typeof(KustoQueryableExtensions).GetMethod(nameof(ProjectAway), BindingFlags.Public | BindingFlags.Static);

    private static readonly MethodInfo SummarizeTemplateMethodWith2Parameters = typeof(KustoQueryableExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.Name == nameof(Summarize) && m.IsGenericMethod && m.GetGenericArguments().Length == 2);

    private static readonly MethodInfo SummarizeTemplateMethodWith4Parameters = typeof(KustoQueryableExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.Name == nameof(Summarize) && m.IsGenericMethod && m.GetGenericArguments().Length == 4);

    [DynamicallyInvoked]
    public static IQueryable<TResult> Summarize<TSource, TBy, TSummary, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, TBy>> bySelector, Expression<Func<IQueryable<TSource>, TSummary>> summarySelector,
        Expression<Func<TBy, TSummary, TResult>> merger)
    {
        var bySelectorExp = bySelector;
        var summarySelectorExp = summarySelector;

        return source.Provider.CreateQuery<TResult>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Summarize, source, bySelector, summarySelector, merger), source.Expression,
            bySelectorExp, summarySelectorExp, merger));
    }

    [DynamicallyInvoked]
    public static IQueryable<TSummary> Summarize<TSource, TSummary>(this IQueryable<TSource> source,
        Expression<Func<IQueryable<TSource>, TSummary>> summarySelector)
    {
        return source.Provider.CreateQuery<TSummary>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Summarize, source, summarySelector),
            [
                source.Expression,
                summarySelector
            ]));
    }

    [DynamicallyInvoked]
    public static int DistinctCount<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector)
    {
        return source.Provider.Execute<int>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(DistinctCount, source, selector),
            [
                source.Expression,
                selector
            ]));
    }

    [DynamicallyInvoked]
    public static TResult Assign<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> selector)
    {
        return source.Provider.Execute<TResult>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Assign, source, selector),
            [
                source.Expression,
                selector
            ]));
    }

    public static IQueryable BuildSummarize<T>(this IQueryable queryable, IEnumerable<string> dimensions,
        IEnumerable<T> measures) where T : MeasureInfo
    {
        return BuildSummarize(queryable, dimensions, measures, out _);
    }

    public static IQueryable BuildSummarize<T>(this IQueryable queryable, IEnumerable<string> dimensions,
        IEnumerable<T> measures, out Type newQueryableEntityType) where T : MeasureInfo
    {
        var queryableElementType = LinqHelper.GetLinqElementType(queryable);
        var typedParameterExpression = Expression.Parameter(queryableElementType);

        Type dimensionsAnonymousType = null;
        Expression dimensionsExpression = null;

        if (dimensions?.Any() == true)
        {
            dimensionsExpression = BuildExpressionForDimensions(queryableElementType, typedParameterExpression,
                dimensions, out dimensionsAnonymousType);
        }

        var measuresExpression = BuildExpressionForMeasures(queryableElementType, typedParameterExpression, measures,
            out var measuresAnonymousType);

        Type mergeAnonymousType = null;
        Expression mergeExpression = null;

        if (dimensionsAnonymousType != null)
        {
            mergeExpression =
                BuildExpressionForMerge(dimensionsAnonymousType, measuresAnonymousType, out mergeAnonymousType);
        }
        // var expUnaryMeasures = expLambdaMeasures;
        //

        object result;
        if (dimensionsAnonymousType != null)
        {
            var method = SummarizeTemplateMethodWith4Parameters.MakeGenericMethod(queryableElementType,
                dimensionsAnonymousType, measuresAnonymousType, mergeAnonymousType);
            result = method.Invoke(null,
                [queryable, dimensionsExpression, measuresExpression, mergeExpression]);
            newQueryableEntityType = mergeAnonymousType;
        }
        else
        {
            var method =
                SummarizeTemplateMethodWith2Parameters.MakeGenericMethod(queryableElementType, measuresAnonymousType);
            result = method.Invoke(null, [queryable, measuresExpression]);
            newQueryableEntityType = measuresAnonymousType;
        }

        return (IQueryable)result;
    }

    private static Expression BuildExpressionForDimensions(Type queryableElementType,
        ParameterExpression typedParameterExpression, IEnumerable<string> dimensions, out Type anonymousType)
    {
        var properties = dimensions.Select(x => queryableElementType.GetProperty(x)).ToList();
        var dynamicProperties = properties.ConvertAll(x => new Tuple<string, Type>(x.Name, x.PropertyType));
        anonymousType = AnonymousTypeBuilder.BuildAnonymousType(dynamicProperties);

        var propertyExpressionExpressions =
            properties.ConvertAll(x => Expression.Property(typedParameterExpression, x));

        var constructor = anonymousType.GetConstructorOfAnonymousType();
        var newExpression = Expression.New(constructor, propertyExpressionExpressions);

        // return Expression.MakeUnary(ExpressionType.Quote, lambda)
        return Expression.Lambda(newExpression, typedParameterExpression);
    }

    private static Expression BuildExpressionForMeasures<T>(Type queryableElementType,
        ParameterExpression typedParameterExpression, IEnumerable<T> measures, out Type anonymousType)
        where T : MeasureInfo
    {
        // var properties = measures.Select(x => new Tuple<PropertyInfo, AggregationType>(queryableElementType.GetProperty(x.MeasurePropertyName), x.AggregationType)).ToList();
        var queryableTypedParameterExpression =
            Expression.Parameter(typeof(IQueryable<>).MakeGenericType(queryableElementType));

        var dynamicProperties = new List<Tuple<string, Type>>();
        var methodCallExpressions = new List<MethodCallExpression>();

        foreach (var measure in measures)
        {
            var aggregationType = measure.AggregationType;
            var property = queryableElementType.GetProperty(measure.PropertyName);
            var propertyType = property.PropertyType;

            var method = GetAggregateMethodInfo(aggregationType, queryableElementType, propertyType);
            // method = method.MakeGenericMethod(queryableElementType);

            // dynamicProperties.Add(new Tuple<string, Type>(propertyName, method.ReturnType));
            dynamicProperties.Add(new Tuple<string, Type>(measure.TargetPropertyName, method.ReturnType));

            //if (aggregationType == AggregationType.Count)
            //{
            //    methodCallExpression = Expression.Call(method, queryableTypedParameterExpression);
            //}
            //else
            //{
            var propertyExpression = Expression.Property(typedParameterExpression, property);
            // var lambda = Expression.Lambda(propertyExpression, Expression.Parameter(typeof(IQueryable<>).MakeGenericType(measuresProperty.Item1.PropertyType)));
            var unaryExpression = Expression.Lambda(propertyExpression, typedParameterExpression);
            // methodCallExpression = Expression.Call(method, queryableTypedParameterExpression);
            var methodCallExpression = Expression.Call(method, queryableTypedParameterExpression, unaryExpression);

            //}
            methodCallExpressions.Add(methodCallExpression);
        }

        anonymousType = AnonymousTypeBuilder.BuildAnonymousType(dynamicProperties);

        var members = new List<MemberInfo>();

        foreach (var dp in dynamicProperties)
        {
            members.Add(anonymousType.GetProperty(dp.Item1));
        }

        var constructor = anonymousType.GetConstructorOfAnonymousType();
        var newExpression = Expression.New(constructor, methodCallExpressions.ToArray(), members);
        return Expression.Lambda(newExpression, queryableTypedParameterExpression);
        // return Expression.MakeUnary(ExpressionType.Quote, finalLambda);
    }

    private static Expression BuildExpressionForMerge(Type dimensionsAnanumousType, Type measuresAnonymousType,
        out Type anonymousType)
    {
        var dimensionsTypedParameterExpression = Expression.Parameter(dimensionsAnanumousType);
        var measuresTypedParameterExpression = Expression.Parameter(measuresAnonymousType);

        var dynamicProperties = new List<Tuple<string, Type>>();
        var propertyExpressions = new List<MemberExpression>();

        foreach (var property in dimensionsAnanumousType.GetProperties().Where(p => p.GetIndexParameters().Length == 0))
        {
            var propertyExpression = Expression.Property(dimensionsTypedParameterExpression, property);
            propertyExpressions.Add(propertyExpression);
            dynamicProperties.Add(new Tuple<string, Type>(property.Name, property.PropertyType));
        }

        foreach (var property in measuresAnonymousType.GetProperties().Where(p => p.GetIndexParameters().Length == 0))
        {
            var propertyExpression = Expression.Property(measuresTypedParameterExpression, property);
            propertyExpressions.Add(propertyExpression);
            dynamicProperties.Add(new Tuple<string, Type>(property.Name, property.PropertyType));
        }

        anonymousType = AnonymousTypeBuilder.BuildAnonymousType(dynamicProperties);

        var constructor = anonymousType.GetConstructorOfAnonymousType();
        var newExpression = Expression.New(constructor, propertyExpressions.ToArray());

        if (dimensionsTypedParameterExpression != null)
        {
            return Expression.Lambda(newExpression, dimensionsTypedParameterExpression,
                measuresTypedParameterExpression);
        }

        return Expression.Lambda(newExpression, measuresTypedParameterExpression);
    }

    private static MethodInfo GetAggregateMethodInfo(AggregationType aggregateType, Type queryableElementType,
        Type dataType)
    {
        //if (aggregateType == AggregationType.Count)
        //{
        //    return QueryableCountTemplateMethod.MakeGenericMethod(queryableElementType);
        //}

        var secondParameterType =
            typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(queryableElementType, dataType));
        var aggregate = aggregateType.ToString();
        List<MethodInfo> methods = null;

        if (aggregateType == AggregationType.DistinctCount)
        {
            methods =
            [
                typeof(KustoQueryableExtensions).GetMethod(nameof(DistinctCount),
                    BindingFlags.Public | BindingFlags.Static)
            ];
        }
        else
        {
            methods = typeof(Queryable).GetMethods().Where(method => method.Name == aggregate).ToList();
        }

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 2)
            {
                continue;
            }

            if (parameters[1].ParameterType.IsGenericType)
            {
                var genericTypeDefinition = parameters[1].ParameterType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IComparable<>) ||
                    genericTypeDefinition == typeof(IComparer<>))
                {
                    continue;
                }
            }

            var funcGenericArguments = parameters[1].ParameterType.GetGenericArguments()[0].GetGenericArguments();

            if (funcGenericArguments.Length > 1 && funcGenericArguments[1] == dataType)
            {
                return method.MakeGenericMethod(queryableElementType);
            }

            if (aggregateType is AggregationType.Min or AggregationType.Max or AggregationType.DistinctCount)
            {
                return method.MakeGenericMethod(queryableElementType, dataType);
            }
            // return method;
        }

        return null;
    }

    public static IQueryable<DatePartAndMeasureValue> DatePartSummarize(this IQueryable source, string dateTimeProperty,
        string measureProperty, string datePart, AggregationType aggregateType)
    {
        var targetElementType = typeof(DatePartAndMeasureValue);

        var members = new List<MemberInfo>
        {
            targetElementType.GetProperty(nameof(DatePartAndMeasureValue.DatePart)),
            targetElementType.GetProperty(nameof(DatePartAndMeasureValue.MeasureValue))
        };

        // var queryableTypedParameterExpression = Expression.Parameter(typeof(IQueryable<>).MakeGenericType(typeof(DatePartAndMeasureValue)));

        var queryableElementType = LinqHelper.GetLinqElementType(source);
        var typedParameterExpression = Expression.Parameter(queryableElementType);
        var dateTimePropertyExpression =
            Expression.Property(typedParameterExpression, queryableElementType.GetProperty(dateTimeProperty));

        var measurePropertyExpression =
            Expression.Convert(
                Expression.Property(typedParameterExpression, queryableElementType.GetProperty(measureProperty)),
                typeof(double));

        var datePartLambda = Expression.Call(dateTimePropertyExpression,
            typeof(DateTime).GetMethod(nameof(DateTime.ToString), [typeof(string)]),
            Expression.Constant(datePart));

        var constructor = targetElementType.GetConstructorOfAnonymousType();
        var newExpression = Expression.New(constructor, [datePartLambda, measurePropertyExpression],
            members);
        var finalLambda = Expression.Lambda(newExpression, typedParameterExpression);

        var method =
            SharedReflectionInfo.QueryableSelectTemplateMethod.MakeGenericMethod(queryableElementType,
                targetElementType);
        var result = method.Invoke(null, [source, finalLambda]);
        // var finalQueryable = KustoQueryableExtensions.BuildSummarize(result as IQueryable, new List<string> { nameof(DatePartAndMeasureValue.DatePart) }, new List<Tuple<string, AggregationType>> { new Tuple<string, AggregationType>(nameof(DatePartAndMeasureValue.MeasureValue), aggregateType) }, ref targetElementType);

        var newSource = result as IQueryable<DatePartAndMeasureValue>;

        return source.Provider.CreateQuery<DatePartAndMeasureValue>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(DatePartSummarize, newSource, dateTimeProperty, measureProperty, datePart,
                aggregateType), newSource.Expression, Expression.Constant(dateTimeProperty),
            Expression.Constant(measureProperty), Expression.Constant(datePart), Expression.Constant(aggregateType)));
    }

    public static IList EvaluateBagUnpack<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<TResult>>> bagSelector)
    {
        return source.Provider.Execute<IList>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(EvaluateBagUnpack, source, bagSelector),
            [
                source.Expression,
                bagSelector
            ]));
    }

    public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
        IQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector,
        JoinKind joinKind)
    {
        return outer.Provider.CreateQuery<TResult>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(Join, outer, inner, outerKeySelector, innerKeySelector, resultSelector,
                joinKind), outer.Expression, inner.Expression, outerKeySelector, innerKeySelector, resultSelector,
            Expression.Constant(joinKind)));
    }

    [DynamicallyInvoked]
    public static IQueryable<TSource> MultiOrderBy<TSource>(this IQueryable<TSource> source,
        params PropertySortingInfo[] sortingInfos)
    {
        return source.Provider.CreateQuery<TSource>(Expression.Call(null,
            LinqFakeMethods.GetMethodInfo(MultiOrderBy, source, sortingInfos),
            [
                source.Expression,
                Expression.Constant(sortingInfos)
            ]));
    }

    public static IQueryable MultiOrderBy(this IQueryable source, SortingOrder sortingOrder, params string[] properties)
    {
        if (properties == null || properties.Length == 0)
        {
            return source;
        }

        var sortingInfos = new PropertySortingInfo[properties.Length];

        for (var i = 0; i < properties.Length; i++)
            sortingInfos[i] = new PropertySortingInfo(properties[i], sortingOrder);

        return MultiOrderBy(source, sortingInfos);
    }

    public static IQueryable MultiOrderBy(this IQueryable source, params PropertySortingInfo[] sortingInfos)
    {
        if (sortingInfos == null || sortingInfos.Length == 0)
        {
            return source;
        }

        var method = MultiOrderByTemplateMethod.MakeGenericMethod(LinqHelper.GetLinqElementType(source));
        return (IQueryable)method.Invoke(null, [source, sortingInfos]);
    }

    public static TSource MVExpand<TSource>(this PackedList<TSource> source)
    {
        throw new NotImplementedException();
    }

    public static IQueryable<TTo> ProjectAway<TFrom, TTo>(this IQueryable<TFrom> source,
        params string[] propertiesToDrop)
    {
        var projectAwayMethod = ProjectAwayTemplateMethod.MakeGenericMethod(typeof(TFrom), typeof(TTo));
        return source.Provider.CreateQuery<TTo>(Expression.Call(null, projectAwayMethod,
        [
            source.Expression,
            Expression.Constant(propertiesToDrop)
        ]));
    }

    public static Expression<Func<IQueryable<TSource>, TSource>> ToScalar<TSource>(this IQueryable<TSource> source)
    {
        var callExp = Expression.Call(null, LinqFakeMethods.GetMethodInfo(ToScalar, source), source.Expression);

        var parameter = Expression.Parameter(typeof(IQueryable<TSource>));
        return Expression.Lambda<Func<IQueryable<TSource>, TSource>>(callExp, parameter);
    }

    // public static IQueryable<TResult> MVExpand<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> mapper)
    // {
    //    var method = GetMethodInfo(MVExpand, source, mapper);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //        source.Expression,
    //        mapper
    //    }));
    // }

    //public static IQueryable<TSource> DistinctBy<TSource, TBy>(this IQueryable<TSource> source, Expression<Func<TSource, TBy>> bySelector)
    //{
    //    return source.Provider.CreateQuery<TSource>(Expression.Call(null, GetMethodInfo(DistinctBy, source, bySelector), new Expression[]
    //    {
    //        source.Expression,
    //        bySelector
    //    }));
    //}

    // public static IQueryable<TResult> ToDynamic<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> mapper)
    // {
    //    throw new NotImplementedException();
    // }
    // public static IQueryable<TResult> MVExpand<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, object>>[] getters)
    // {
    //    var method = GetMethodInfoX(MVExpand, source, getters);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //                source.Expression,
    //                Expression.Constant(getters)
    //    }));
    // }
    // public static IQueryable<TResult> MVExpand<TSource, TResult>(this IQueryable<TSource> source,  params Expression<Func<TSource, object>>[] getters)
    // {
    //    var method = typeof(KustoQueryableExtensions).GetMethod(nameof(MVExpandCore), BindingFlags.NonPublic| BindingFlags.Static).MakeGenericMethod(typeof(TSource), typeof(TResult));
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //                source.Expression,
    //                Expression.Constant(getters)
    //    }));
    // }
    // private static IQueryable<TResult> MVExpandCore<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, object>>[] getters)
    // {
    //   throw new NotImplementedException();
    // }

    // public static IQueryable BuildOrderBy<TSource, TKey>(this IQueryable source, string[] properties)
    // {
    //    return source.Provider.CreateQuery<TSource>(Expression.Call(null, GetMethodInfo(BuildOrderBy, source, keySelector), new Expression[]
    //    {
    //        source.Expression,
    //        Expression.Constant(keySelector)
    //    }));
    // }

    //[DynamicallyInvokable]
    //private static IQueryable<TSource> AppendQueryLiteralCore<TSource>(this IQueryable<TSource> source, string queryLiteral)
    //{
    //    return source.Provider.CreateQuery<TSource>(Expression.Call(null, GetMethodInfo(AppendQueryLiteral, source, queryLiteral), new Expression[]
    //    {
    //        source.Expression,
    //        Expression.Constant(queryLiteral)
    //    }));
    //}

    //public static IQueryable AppendQueryLiteral(this IQueryable source, string queryLiteral)
    //{
    //    var elementType = LinqHelper.GetLinqElementType(source.ElementType);
    //    var method = typeof(KustoQueryableExtensions).GetMethod(nameof(AppendQueryLiteralCore), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(elementType);
    //    var queryable = method.Invoke(null, new object[] { source, queryLiteral });
    //    return (IQueryable)queryable;
    //}

    //public static IQueryable<TSource> AppendQueryLiteral<TSource>(this IQueryable<TSource> source, string queryLiteral)
    //{
    //    return AppendQueryLiteralCore(source, queryLiteral);
    //}
}