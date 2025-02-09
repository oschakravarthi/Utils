using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq;

public sealed class CachedQueryableDecorator<T> : IOrderedQueryable<T>, ICloneableQueryable
{
    private readonly IQueryable<T> actual;

    public CachedQueryableDecorator(IQueryable<T> actual, ICacheKeyBuilder cacheKeyBuilder, IMemoryCache cache,
        TimeOnly absoluteExpirationInUtc)
    {
        this.actual = actual;
        Provider = new CachedQueryProviderDecorator(actual.Provider, cacheKeyBuilder, cache, absoluteExpirationInUtc);
    }

    internal CachedQueryableDecorator(IQueryable<T> actual, IQueryProvider provider)
    {
        this.actual = actual;
        Provider = provider;
    }

    public ICloneableQueryable Clone(Expression expression)
    {
        if (actual is not ICloneableQueryable cloneable)
        {
            throw new InvalidOperationException("This operation is not supported");
        }

        var cloned = (IQueryable<T>)cloneable.Clone(expression);
        return new CachedQueryableDecorator<T>(cloned, Provider);
    }

    public Type ElementType => actual.ElementType;

    public Expression Expression => actual.Expression;

    public IQueryProvider Provider { get; }

    public IEnumerator<T> GetEnumerator()
    {
        return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
    }
}