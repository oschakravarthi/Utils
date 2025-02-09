using Microsoft.Extensions.Caching.Memory;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Compression.LZ4;
using SubhadraSolutions.Utils.Linq.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq;

public sealed class CachedQueryProviderDecorator(IQueryProvider actual, ICacheKeyBuilder cacheKeyBuilder,
        IMemoryCache cache,
        TimeOnly absoluteExpirationInUtc)
    : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
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

    public object Execute(Expression expression)
    {
        var elementType = LinqHelper.GetLinqElementType(expression.Type);

        var methodInfo = GetType().GetMethod(nameof(ExecuteCore), BindingFlags.NonPublic | BindingFlags.Instance);
        var concreteMethod = methodInfo.MakeGenericMethod(elementType);
        return concreteMethod.Invoke(this, [expression]);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return ExecuteCore<TResult>(expression);
    }

    [DynamicallyInvoked]
    private IQueryable<T> CreateQueryCore<T>(Expression expression)
    {
        var query = actual.CreateQuery(expression);
        return new CachedQueryableDecorator<T>((IQueryable<T>)query, this);
    }

    private TResult ExecuteCore<TResult>(Expression expression)
    {
        var visitor = new DecorationRemovedExpressionVisitor();
        expression = visitor.Visit(expression);
        var key = cacheKeyBuilder.BuildKey(expression);
        TResult result = default;
        var slidingExpiry = GetSlidingExpiry(expression, out var cacheMethodExists);
        if (cacheMethodExists && (slidingExpiry == null || slidingExpiry.Value == TimeSpan.Zero))
        {
            result = actual.Execute<TResult>(expression);
            return result;
        }

        var bytes = cache.GetOrCreate(key, c => ExecuteActualAndCache(expression, c, slidingExpiry, key, out result));

        if (result != null)
        {
            return result;
        }

        if (bytes == null)
        {
            return default;
        }

        return bytes.DecompressAndDeserialize<TResult>();
    }

    private byte[] ExecuteActualAndCache<TResult>(Expression expression, ICacheEntry cacheEntry,
        TimeSpan? slidingExpiration, string key, out TResult result)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine("Caching: " + key);
#endif
        result = actual.Execute<TResult>(expression);
        return CacheResult(result, cacheEntry, slidingExpiration);
    }

    private byte[] CacheResult<TResult>(TResult result, ICacheEntry cacheEntry, TimeSpan? slidingExpiration)
    {
        SetAbsoluteExpiry(cacheEntry);
        if (slidingExpiration != null)
        {
            cacheEntry.SlidingExpiration = slidingExpiration;
        }

        if (result == null)
        {
            cacheEntry.Size = 0;
            return null;
        }

        var bytes = result.SerializeAndCompress();
        cacheEntry.Size = bytes.LongLength;

        return bytes;
    }

    private void SetAbsoluteExpiry(ICacheEntry cacheEntry)
    {
        var ts = absoluteExpirationInUtc.ToTimeSpan();
        var now = DateTimeOffset.Now;
        var expiry = new DateTimeOffset(now.Date, ts);
        if (expiry <= now)
        {
            expiry = expiry.AddDays(1);
        }

        cacheEntry.AbsoluteExpiration = expiry;
    }

    private static TimeSpan? GetSlidingExpiry(Expression expression, out bool cacheMethodExists)
    {
        cacheMethodExists = false;
        var callChain = LinqHelper.ExploreCallChainExpressions(expression);
        foreach (var methodCallExp in callChain)
        {
            var method = methodCallExp.Method;
            if (method.DeclaringType == typeof(QueryableExtensions) && method.Name == nameof(QueryableExtensions.Cache))
            {
                if (methodCallExp.Arguments[1] is ConstantExpression timespanExp)
                {
                    cacheMethodExists = true;
                    return (TimeSpan?)timespanExp.Value;
                }
            }
        }

        return null;
    }
}