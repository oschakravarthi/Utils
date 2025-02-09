using Remote.Linq;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Expressions;
using SubhadraSolutions.Utils.ApiClient.Helpers;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Validation;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient;

public sealed class SmartAsyncRemoteQueryProvider<TSource> : IAsyncRemoteQueryProvider
{
    private static readonly MethodInfo _executeMethod = typeof(SmartAsyncRemoteQueryProvider<TSource>)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance).First(m => m.Name == nameof(Execute) && m.IsGenericMethod);

    private static readonly MethodInfo createQueryCoreMethodInfo = typeof(SmartAsyncRemoteQueryProvider<TSource>).GetMethod(nameof(CreateQueryCore), BindingFlags.NonPublic | BindingFlags.Instance);

    private readonly Func<string, Expression, CancellationToken, ValueTask<TSource>> _asyncDataProvider;

    private readonly IExpressionToRemoteLinqContext _context;
    private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
    private readonly string url;

    public SmartAsyncRemoteQueryProvider(string url, Func<string, Expression, CancellationToken, ValueTask<TSource>> asyncDataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper, IExpressionToRemoteLinqContext context)
    {
        Guard.ArgumentShouldNotBeNull(asyncDataProvider, nameof(asyncDataProvider));
        Guard.ArgumentShouldNotBeNull(resultMapper, nameof(resultMapper));
        this.url = url;
        _asyncDataProvider = asyncDataProvider;
        _resultMapper = resultMapper;
        _context = context;
    }

    /// <inheritdoc />
    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
    {
        return CreateQueryCore<TElement>(expression);
    }

    /// <inheritdoc />
    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
    {
        var elementType = LinqHelper.GetLinqElementType(expression.Type);

        var concreteMethod = createQueryCoreMethodInfo.MakeGenericMethod(elementType);
        var obj = concreteMethod.Invoke(this, [expression]);
        return (IQueryable)obj;

        //return new SmartAsyncRemoteQueryable(TypeHelper.GetElementType(expression.CheckNotNull("expression").Type) ?? throw new RemoteLinqException($"Failed to get element type of {expression.Type}"), this, expression);
    }

    /// <inheritdoc />
    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
    {
        try
        {
            return ExecuteAsync<TResult>(expression, CancellationToken.None).AsTask().Result;
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }

            throw;
        }
    }

    /// <inheritdoc />
    public object Execute(System.Linq.Expressions.Expression expression)
    {
        return this.InvokeAndUnwrap<object>(_executeMethod, expression);
    }

    /// <inheritdoc />
    public async ValueTask<TResult> ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression,
        CancellationToken cancellation)
    {
        ExpressionHelper.CheckExpressionResultType<TResult>(expression);
        var rlinq = expression.TranslateExpression();
        var source = await _asyncDataProvider(this.url, rlinq, cancellation).ConfigureAwait(false);
        return await _resultMapper.MapResultAsync<TResult>(source, expression, cancellation).ConfigureAwait(false);
    }

    private IQueryable<TElement> CreateQueryCore<TElement>(System.Linq.Expressions.Expression expression)
    {
        return new SmartAsyncRemoteQueryable<TElement>(this, expression);
    }
}