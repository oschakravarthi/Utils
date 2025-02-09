using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Json;
using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Caching.Redis;

/// <summary>
///     <seealso cref="ICache" /> implementation for <seealso cref="RedisConnection" />.
/// </summary>
public class RedisCache(RedisConnection redisConnection) : AbstractDisposable, ICache
{
    public void Clear()
    {
        //TODO
        throw new NotImplementedException();
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="keyPrefix">
    ///     Optional: A custom prefix used with the <see cref="key" /> param. If not provided, calling file
    ///     name will be prepended.
    /// </param>
    /// <returns>The deserialized JSON object from cache.</returns>
    public T Get<T>(string key)
    {
        return JsonSerializationHelper.Deserialize<T>(redisConnection.BasicRetry(db => db.StringGet(key)));
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="keyPrefix">
    ///     Optional: A custom prefix used with the <see cref="key" /> param. If not provided, calling file
    ///     name will be prepended.
    /// </param>
    /// <returns>The deserialized JSON object from cache.</returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return JsonSerializationHelper.Deserialize<T>(await redisConnection
            .BasicRetryAsync(async db => await db.StringGetAsync(key).ConfigureAwait(false)).ConfigureAwait(false));
    }

    public T GetOrAdd<T>(string key, Func<string, ValueAndExpiry<T>> factory)
    {
        //TODO:
        throw new NotImplementedException();
    }

    public Task<T> GetOrAddAsync<T>(string key, Func<string, ValueAndExpiry<T>> factory)
    {
        //TODO:
        throw new NotImplementedException();
    }

    /// <summary>
    /// Caches the value, overwriting any previous value with this key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiry">Optional: Cached value will expire after the interval.</param>
    // <param name="keyPrefix">Optional: A custom prefix used with the <see cref="key"/> param. If not provided, calling file name will be prepended.</param>
    /// <returns>True if value was successfully cached. Otherwise, false.</returns>
    public bool Set<T>(string key, T value, TimeSpan? expiry = null)
    {
        return redisConnection.BasicRetry(db => db.StringSet(key, JsonSerializationHelper.Serialize(value), expiry));
    }

    /// <summary>
    ///     Caches the value, overwriting any previous value with this key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiry">Optional: Cached value will expire after the interval.</param>
    /// <param name="keyPrefix">
    ///     Optional: A custom prefix used with the <see cref="key" /> param. If not provided, calling file
    ///     name will be prepended.
    /// </param>
    /// <returns>True if value was successfully cached. Otherwise, false.</returns>
    public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        return redisConnection.BasicRetryAsync(async db =>
                await db.StringSetAsync(key, JsonSerializationHelper.Serialize(value), expiry).ConfigureAwait(false))
;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="keyPrefix">
    ///     Optional: A custom prefix used with the <see cref="key" /> param. If not provided, calling file
    ///     name will be prepended.
    /// </param>
    /// <returns>True if value was successfully retrieved. Otherwise, false.</returns>
    public bool TryGet<T>(string key, out T value)
    {
        try
        {
            value = Get<T>(key);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            redisConnection?.Dispose();
        }
        catch
        {
        }
    }
}