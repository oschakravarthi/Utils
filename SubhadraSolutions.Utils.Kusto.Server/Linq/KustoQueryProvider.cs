using Kusto.Data.Common;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal class KustoQueryProvider : IQueryProvider, IAsyncQueryProvider
{
    internal readonly QueryContext queryContext;

    internal KustoQueryProvider(ICslQueryProvider cslQueryProvider, QueryContext queryContext)
    {
        CslQueryProvider = cslQueryProvider;
        this.queryContext = queryContext;
    }

    public ICslQueryProvider CslQueryProvider { get; }

    public async ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
    {
        var visitorQuery = BuildQuery(expression, queryContext);

        var elementType = LinqHelper.GetLinqElementType(expression.Type);
        var entityBuilderFactory = typeof(DynamicEntityBuilderFactory<>).MakeGenericType(elementType)
            .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        var enumerableType = typeof(KustoEntitiesAsyncEnumerable<>).MakeGenericType(elementType);

        var enumerable = Activator.CreateInstance(enumerableType, this.CslQueryProvider, visitorQuery,
            queryContext.ClientRequestProperties, entityBuilderFactory);

        if (DataRecordHelper.GetReaderMethod(typeof(TResult)) != null)
        {
            var e = ((IAsyncEnumerable<TResult>)enumerable).GetAsyncEnumerator(token);
            await e.MoveNextAsync().ConfigureAwait(false);
            return e.Current;
        }

        var typeToReturn = typeof(TResult);

        if (typeToReturn.IsInstanceOfType(enumerable))
        {
            return (TResult)enumerable;
        }

        if (typeof(IEnumerable<>).MakeGenericType(typeToReturn).IsInstanceOfType(enumerable))
        {
            var enumerator = ((IEnumerable)enumerable).GetEnumerator();

            if (enumerator.MoveNext())
            {
                return (TResult)enumerator.Current;
            }
        }

        if (expression is MethodCallExpression callExp && callExp.Method.DeclaringType == typeof(Queryable))
        {
            if (callExp.Method.Name is nameof(Queryable.FirstOrDefault) or nameof(Queryable.First) or nameof(Queryable.Single) or nameof(Queryable.SingleOrDefault))
            {
                var enumerator = ((IAsyncEnumerable<TResult>)enumerable).GetAsyncEnumerator(token);

                if (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    return enumerator.Current;
                }

                if (callExp.Method.Name is nameof(Queryable.First) or nameof(Queryable.Single))
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                return default;
            }
        }

        return (TResult)enumerable;
    }

    IAsyncQueryable<TElement> IAsyncQueryProvider.CreateQuery<TElement>(Expression expression)
    {
        return CreateQueryCore<TElement>(expression);
    }

    public virtual IQueryable CreateQuery(Expression expression)
    {
        var elementType = LinqHelper.GetLinqElementType(expression.Type);

        var methodInfo = GetType().GetMethod(nameof(CreateQueryCore), BindingFlags.NonPublic | BindingFlags.Instance);
        var concreteMethod = methodInfo.MakeGenericMethod(elementType);
        var obj = concreteMethod.Invoke(this, [expression]);
        return (IQueryable)obj;
    }

    public IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return CreateQueryCore<T>(expression);
    }

    public T Execute<T>(Expression expression)
    {
        return ExecuteCore<T>(expression);
    }

    object IQueryProvider.Execute(Expression expression)
    {
        try
        {
            var elementType = LinqHelper.GetLinqElementType(expression.Type);

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            if (enumerableType.IsAssignableFrom(expression.Type))
            {
                elementType = enumerableType;
            }

            var methodInfo = GetType().GetMethod(nameof(ExecuteCore), BindingFlags.NonPublic | BindingFlags.Instance);
            var concreteMethod = methodInfo.MakeGenericMethod(elementType);

            return concreteMethod.Invoke(this, [expression]);
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException;
        }
    }

    public static string BuildQuery(IQueryPart queryPart)
    {
        var setQueryParts = new List<IQueryPart>();
        queryPart.PopulateSetQueryParts(setQueryParts);
        for (var i = 0; i < setQueryParts.Count; i++)
        {
            var p = (SetQueryPart)setQueryParts[i];
            p.Name = "set" + (i + 1);
        }

        var visited = new HashSet<IQueryPart>();
        var sb = new StringBuilder();

        foreach (var setQueryPart in setQueryParts)
        {
            if (!visited.Contains(setQueryPart))
            {
                visited.Add(setQueryPart);
                sb.AppendLine(((SetQueryPart)setQueryPart).GetQueryWithSetDefinition());
            }
        }

        var visitorQuery = queryPart.GetQuery();
        return sb + visitorQuery;
    }

    public static string BuildQuery(Expression expression, QueryContext queryContext)
    {
        return BuildQuery(expression, queryContext, out var _);
    }

    public static string BuildQuery(Expression expression, QueryContext queryContext,
        out QueryPartExpression querypartExpression)
    {
        var visitor = new KustoExpressionVisitor(queryContext);
        var part = visitor.Visit(expression);

        var partExp = (QueryPartExpression)part;
        querypartExpression = partExp;
        var query = BuildQuery(partExp.QueryPart);
        var parametersHeader = queryContext.BuildQueryParametersHeader();
        if (parametersHeader == null)
        {
            return query;
        }

        return parametersHeader + query;
    }

    public string BuildQuery(Expression expression)
    {
        return BuildQuery(expression, queryContext);
    }

    public string BuildQuery(Expression expression, out QueryPartExpression queryPartExpression)
    {
        return BuildQuery(expression, queryContext, out queryPartExpression);
    }

    public KustoQueryable<T> CreateQuery<T>(string query)
    {
        return new KustoQueryable<T>(this.CslQueryProvider, query, null, queryContext, this);
    }

    private IList BuildAnonymousList(string query, ClientRequestProperties clientRequestProperties)
    {
        var enumerator = KustoHelper.BuildAnonymousEnumerator(this.CslQueryProvider, query, clientRequestProperties,
            out var dtoType);
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dtoType));
        while (enumerator.MoveNext()) list.Add(enumerator.Current);
        return list;
    }

    [DynamicallyInvoked]
    private KustoQueryable<T> CreateQueryCore<T>(Expression expression)
    {
        return new KustoQueryable<T>(this.CslQueryProvider, null, expression, queryContext, this);
    }

    [DynamicallyInvoked]
    private T ExecuteCore<T>(Expression expression)
    {
        var visitorQuery = BuildQuery(expression, out var queryPartExpression);
#if DEBUG
        System.Diagnostics.Debug.WriteLine("____________________________________________");
        //this.queryContext.PrintParameters();
        System.Diagnostics.Debug.WriteLine(visitorQuery);
        System.Diagnostics.Debug.WriteLine("____________________________________________");
#endif
        var typeToReturn = typeof(T);
        if (typeToReturn == typeof(IList))
        {
            return (T)BuildAnonymousList(visitorQuery, queryContext.ClientRequestProperties);
        }

        var elementType = LinqHelper.GetLinqElementType(expression.Type);
        var entityBuilderFactory = typeof(DynamicEntityBuilderFactory<>).MakeGenericType(elementType)
            .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        var enumerableType = typeof(KustoEntitiesEnumerable<>).MakeGenericType(elementType);

        var enumerable = Activator.CreateInstance(enumerableType, this.CslQueryProvider, visitorQuery,
            queryContext.ClientRequestProperties, entityBuilderFactory);

        if (DataRecordHelper.GetReaderMethod(typeof(T)) != null)
        {
            var e = ((IEnumerable)enumerable).GetEnumerator();
            e.MoveNext();
            return (T)e.Current;
        }

        if (typeToReturn.IsInstanceOfType(enumerable))
        {
            return (T)enumerable;
        }

        if (typeof(IEnumerable<>).MakeGenericType(typeToReturn).IsInstanceOfType(enumerable))
        {
            var enumerator = ((IEnumerable)enumerable).GetEnumerator();

            if (enumerator.MoveNext())
            {
                return (T)enumerator.Current;
            }
        }

        if (expression is MethodCallExpression callExp && callExp.Method.DeclaringType == typeof(Queryable))
        {
            if (callExp.Method.Name is nameof(Queryable.FirstOrDefault) or nameof(Queryable.First) or nameof(Queryable.Single) or nameof(Queryable.SingleOrDefault))
            {
                var enumerator = ((IEnumerable)enumerable).GetEnumerator();

                if (enumerator.MoveNext())
                {
                    return (T)enumerator.Current;
                }

                if (callExp.Method.Name is nameof(Queryable.First) or nameof(Queryable.Single))
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                return default;
            }
        }

        return (T)enumerable;
    }
}