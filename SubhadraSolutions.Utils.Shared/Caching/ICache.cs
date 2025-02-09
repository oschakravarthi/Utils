using SubhadraSolutions.Utils.Contracts;
using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Caching;

public interface ICache : IDisposable, IClearable
{
    T Get<T>(string key);

    Task<T> GetAsync<T>(string key);

    T GetOrAdd<T>(string key, Func<string, ValueAndExpiry<T>> factory);

    Task<T> GetOrAddAsync<T>(string key, Func<string, ValueAndExpiry<T>> factory);

    bool Set<T>(string key, T value, TimeSpan? expiry = null);

    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    bool TryGet<T>(string key, out T value);
}