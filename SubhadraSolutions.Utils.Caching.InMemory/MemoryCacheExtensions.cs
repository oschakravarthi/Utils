using Microsoft.Extensions.Caching.Memory;
using SubhadraSolutions.Utils.Contracts;
using System;
using System.Reflection;

namespace SubhadraSolutions.Utils.Caching.InMemory;

public static class MemoryCacheExtensions
{
    public static void Clear(this IMemoryCache cache)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache), "Memory cache must not be null");
        }

        if (cache is IClearable clearable)
        {
            clearable.Clear();
            return;
        }

        if (cache is MemoryCache memCache)
        {
            memCache.Compact(1.0);
            return;
        }

        var clearMethod = cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
        if (clearMethod != null)
        {
            clearMethod.Invoke(cache, null);
            return;
        }

        var prop = cache.GetType().GetProperty("EntriesCollection",
            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
        if (prop != null)
        {
            var innerCache = prop.GetValue(cache);
            if (innerCache != null)
            {
                clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                if (clearMethod != null)
                {
                    clearMethod.Invoke(innerCache, null);
                    return;
                }
            }
        }

        throw new InvalidOperationException("Unable to clear memory cache instance of type " +
                                            cache.GetType().FullName);
    }
}