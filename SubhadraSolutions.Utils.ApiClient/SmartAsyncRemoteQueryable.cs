using Remote.Linq;
using Remote.Linq.DynamicQuery;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient;

public class SmartAsyncRemoteQueryable : AsyncRemoteQueryable, ICloneableQueryable
{
    public SmartAsyncRemoteQueryable(Type elemntType, IAsyncRemoteQueryProvider provider,
        Expression expression = null)
        : base(elemntType, provider, expression)
    {
    }

    public ICloneableQueryable Clone(Expression expression)
    {
        return new SmartAsyncRemoteQueryable(ElementType, Provider, expression);
    }
}

public sealed class SmartAsyncRemoteQueryable<T> : AsyncRemoteQueryable, ICloneableQueryable,
    IOrderedAsyncRemoteQueryable<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="T:SmartAsyncRemoteQueryable`1" /> class.
    /// </summary>
    public SmartAsyncRemoteQueryable(IAsyncRemoteQueryProvider provider, Expression expression = null)
        : base(typeof(T), provider, expression)
    {
    }

    public ICloneableQueryable Clone(Expression expression)
    {
        return new SmartAsyncRemoteQueryable<T>(Provider, expression);
    }

    /// <inheritdoc />
    public IEnumerable<T> Execute()
    {
        return Provider.Execute<IEnumerable<T>>(Expression);
    }

    /// <inheritdoc />
    public ValueTask<IEnumerable<T>> ExecuteAsync(CancellationToken cancellation = default)
    {
        return Provider.ExecuteAsync<IEnumerable<T>>(Expression, cancellation);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return Execute().GetEnumerator();
    }
}