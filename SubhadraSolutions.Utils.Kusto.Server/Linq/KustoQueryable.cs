using Kusto.Data.Common;
using SubhadraSolutions.Utils.Kusto.Server.Linq.Expressions;
using SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

public class KustoQueryable<T> : IKustoQueryable<T>
{
    private readonly KustoQueryProvider provider;

    public KustoQueryable(ICslQueryProvider cslQueryProvider, string query) : this(
        cslQueryProvider, query, null, null, null)
    {
    }

    public KustoQueryable(ICslQueryProvider cslQueryProvider, QueryPartExpression expression) : this(
        cslQueryProvider, null, expression, null, null)
    {
    }

    public KustoQueryable(ICslQueryProvider cslQueryProvider, IQueryPart queryPart) : this(
        cslQueryProvider, null, new QueryPartExpression(null, queryPart), null, null)
    {
    }

    internal KustoQueryable(ICslQueryProvider cslQueryProvider, string query, Expression expression,
        QueryContext queryContext, KustoQueryProvider queryProvider)
    {
        if (queryContext == null)
        {
            queryContext = new QueryContext();
        }

        if (expression != null)
        {
            if (expression is QueryPartExpression queryPartExpression)
            {
                Expression = new QueryPartExpression(Expression.Constant(this), queryPartExpression.QueryPart);
            }
            else
            {
                Expression = expression;
            }
        }
        else
        {
            var isTemplate = query.Contains("{0}");
            var setQueryPart = new SetQueryPart(new LiteralQueryPart(query), isTemplate, queryContext);
            Expression = new QueryPartExpression(Expression.Constant(this), setQueryPart);
        }

        if (queryProvider == null)
        {
            queryProvider = new KustoQueryProvider(cslQueryProvider, queryContext);
        }

        provider = queryProvider;
    }

    public Type ElementType => typeof(T);

    public Expression Expression { get; }

    public IQueryProvider Provider => provider;

    Expression IQueryable.Expression => Expression;

    IAsyncQueryProvider IAsyncQueryable.Provider => provider;

    public ICloneableQueryable Clone(Expression expression)
    {
        return new KustoQueryable<T>(provider.CslQueryProvider, null, expression, null, null);
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var enumerable = await provider.ExecuteAsync<IAsyncEnumerable<T>>(Expression, cancellationToken)
            .ConfigureAwait(false);
        var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false)) yield return enumerator.Current;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
    }
}