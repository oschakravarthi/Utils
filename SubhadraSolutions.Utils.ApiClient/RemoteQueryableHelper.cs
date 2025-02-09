using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Expressions;
using SubhadraSolutions.Utils.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient;

public static class RemoteQueryableHelper
{
    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider,
            new AsyncDynamicResultMapper(context?.ValueMapper), context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider,
            new AsyncDynamicResultMapper(context?.ValueMapper), context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, ValueTask<DynamicObject>> dataProvider, IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable<T, DynamicObject>(url, dataProvider,
            new AsyncDynamicResultMapper(context?.ValueMapper), context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable<T>(url, dataProvider,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable<T, DynamicObject>(url, dataProvider,
            new AsyncDynamicResultMapper(context?.ValueMapper), context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable<T>(url, dataProvider,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, ValueTask<object>> dataProvider,
        IAsyncQueryResultMapper<object> resultMapper = null, IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable<object>(url, elementType, dataProvider,
            resultMapper ?? new AsyncObjectResultCaster(), context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, ValueTask<object>> dataProvider, ITypeInfoProvider typeInfoProvider,
        IAsyncQueryResultMapper<object> resultMapper = null,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, CancellationToken, ValueTask<object>> dataProvider,
        IAsyncQueryResultMapper<object> resultMapper = null, IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable<object>(url, elementType, dataProvider,
            resultMapper ?? new AsyncObjectResultCaster(), context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        string url, Type elementType, Func<string, Expression, CancellationToken, ValueTask<object>> dataProvider,
        ITypeInfoProvider typeInfoProvider, IAsyncQueryResultMapper<object> resultMapper = null,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, ValueTask<object>> dataProvider, IAsyncQueryResultMapper<object> resultMapper = null,
        IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable<T, object>(url, dataProvider, resultMapper ?? new AsyncObjectResultCaster(),
            context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, ValueTask<object>> dataProvider, ITypeInfoProvider typeInfoProvider,
        IAsyncQueryResultMapper<object> resultMapper = null,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable<T>(url, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, CancellationToken, ValueTask<object>> dataProvider,
        IAsyncQueryResultMapper<object> resultMapper = null, IExpressionToRemoteLinqContext context = null)
    {
        return CreateAsyncQueryable<T, object>(url, dataProvider, resultMapper ?? new AsyncObjectResultCaster(),
            context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        string url, Func<string, Expression, CancellationToken, ValueTask<object>> dataProvider, ITypeInfoProvider typeInfoProvider,
        IAsyncQueryResultMapper<object> resultMapper = null,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable<T>(url, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    //
    // Type parameters:
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        string url, Type elementType, Func<string, Expression, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, IExpressionToRemoteLinqContext context = null)
    {
        Guard.ArgumentShouldNotBeNull(dataProvider, nameof(dataProvider));
        var dataProvider2 = dataProvider;
        return CreateAsyncQueryable(url, elementType, (u, expression, _) => dataProvider2(u, expression), resultMapper,
            context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    //
    // Type parameters:
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        string url, Type elementType, Func<string, Expression, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    //
    // Type parameters:
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(string url, Type elementType, Func<string, Expression, CancellationToken, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, IExpressionToRemoteLinqContext context = null)
    {
        var provider = new SmartAsyncRemoteQueryProvider<TSource>(url, dataProvider, resultMapper, context);
        return new SmartAsyncRemoteQueryable(elementType, provider);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IRemoteQueryable that utilizes the data provider
    //     specified.
    //
    // Type parameters:
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(string url, Type elementType, Func<string, Expression, CancellationToken, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable(url, elementType, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    //
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        string url, Func<string, Expression, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, IExpressionToRemoteLinqContext context = null)
    {
        Guard.ArgumentShouldNotBeNull(dataProvider, nameof(dataProvider));

        var dataProvider2 = dataProvider;
        return CreateAsyncQueryable<T, TSource>(url, (u, expression, _) => dataProvider2(u, expression), resultMapper,
            context);
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    //
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        string url, Func<string, Expression, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable<T, TSource>(url, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    //
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        string url,
        Func<string, Expression, CancellationToken, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, IExpressionToRemoteLinqContext context = null)
    {
        return new SmartAsyncRemoteQueryable<T>(
            new SmartAsyncRemoteQueryProvider<TSource>(url, dataProvider, resultMapper, context));
    }

    //
    // Summary:
    //     Creates an instance of Remote.Linq.IAsyncRemoteQueryable`1 that utilizes the
    //     data provider specified.
    //
    // Type parameters:
    //   T:
    //     Element type of the Remote.Linq.IAsyncRemoteQueryable`1.
    //
    //   TSource:
    //     Data type served by the data provider.
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        string url,
        Func<string, Expression, CancellationToken, ValueTask<TSource>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider typeInfoProvider,
        Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
    {
        return CreateAsyncQueryable<T, TSource>(url, dataProvider, resultMapper,
            SmartRemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }
}