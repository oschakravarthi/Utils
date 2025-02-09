using Microsoft.Extensions.Caching.Memory;
using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Caching.InMemory;

public class InMemoryCache(IMemoryCache cache) : AbstractDisposable, ICache
{
    public void Clear()
    {
        cache.Clear();
    }

    public T Get<T>(string key)
    {
        return cache.Get<T>(key);
    }

    public Task<T> GetAsync<T>(string key)
    {
        return Task.FromResult(Get<T>(key));
    }

    public T GetOrAdd<T>(string key, Func<string, ValueAndExpiry<T>> factory)
    {
        return cache.GetOrCreate(key, cacheEntry =>
        {
            var valueAndExpiry = factory(key);
            if (valueAndExpiry.Expiry != null)
            {
                cacheEntry.SlidingExpiration = valueAndExpiry.Expiry.Value;
            }

            return valueAndExpiry.Value;
        });
    }

    public Task<T> GetOrAddAsync<T>(string key, Func<string, ValueAndExpiry<T>> factory)
    {
        return Task.FromResult(GetOrAdd(key, factory));
    }

    public bool Set<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (expiry != null)
        {
            cache.Set(key, value, expiry.Value);
        }
        else
        {
            cache.Set(key, value);
        }

        return true;
    }

    public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        return Task.FromResult(Set(key, value, expiry));
    }

    public bool TryGet<T>(string key, out T value)
    {
        return cache.TryGetValue(key, out value);
    }

    protected override void Dispose(bool disposing)
    {
        cache.Dispose();
    }
}