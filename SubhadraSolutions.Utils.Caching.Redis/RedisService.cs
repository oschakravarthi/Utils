using Newtonsoft.Json;
using StackExchange.Redis;
using SubhadraSolutions.Utils.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Caching.Redis;

public class RedisService(string connectionString) : IRedisService
{
    protected readonly RedisConnection RedisConnection = RedisConnection.Initialize(connectionString);

    public virtual bool Delete(string key)
    {
        return RedisConnection.BasicRetry(db => db.KeyDelete(key));
    }

    public virtual Task<bool> DeleteAsync(string key)
    {
        return RedisConnection.BasicRetryAsync(async db => await db.KeyDeleteAsync(key).ConfigureAwait(false));
    }

    public long DeleteKeysByPrefix(string keyPrefix)
    {
        var server = RedisConnection.GetServer();
        var keysToDelete = server.Keys(pattern: $"{keyPrefix}*").ToArray();
        return RedisConnection.BasicRetry(db => db.KeyDelete(keysToDelete));
    }

    public virtual T Get<T>(string key)
    {
        var redisValue = RedisConnection.BasicRetry(db => db.StringGet(key));
        if (redisValue.HasValue)
        {
            return JsonSerializationHelper.Deserialize<T>(redisValue);
        }

        return default;
    }

    public virtual IEnumerable<T> Get<T>(IEnumerable<string> keys)
    {
        var redisKeys = new RedisKey[keys.Count()];
        var i = 0;
        foreach (var key in keys)
        {
            redisKeys[i] = new RedisKey(key);
            i++;
        }

        var redisValues = RedisConnection.BasicRetry(db => db.StringGet(redisKeys));
        if (redisValues.Length > 0)
        {
            var result = new List<T>();
            foreach (var redisValue in redisValues.Where(x => x.HasValue))
            {
                result.Add(JsonSerializationHelper.Deserialize<T>(redisValue));
            }

            return result;
        }

        return default;
    }

    public virtual async Task<T> GetAsync<T>(string key)
    {
        var redisValue = await RedisConnection.BasicRetryAsync(async db => await db.StringGetAsync(key).ConfigureAwait(false)).ConfigureAwait(false);
        if (redisValue.HasValue)
        {
            return JsonSerializationHelper.Deserialize<T>(redisValue);
        }

        return default;
    }

    public virtual async Task<IEnumerable<T>> GetAsync<T>(IEnumerable<string> keys)
    {
        var redisKeys = new RedisKey[keys.Count()];
        var i = 0;
        foreach (var key in keys)
        {
            redisKeys[i] = new RedisKey(key);
            i++;
        }

        var redisValues = await RedisConnection.BasicRetryAsync(async db => await db.StringGetAsync(redisKeys).ConfigureAwait(false)).ConfigureAwait(false);
        if (redisValues.Length > 0)
        {
            var result = new List<T>();
            foreach (var redisValue in redisValues.Where(x => x.HasValue))
            {
                result.Add(JsonSerializationHelper.Deserialize<T>(redisValue));
            }

            return result;
        }

        return default;
    }

    public virtual void Set<T>(string key, T value, TimeSpan? expiry = null)
    {
        RedisConnection.BasicRetry(db => db.StringSet(key, JsonConvert.SerializeObject(value), expiry));
    }

    public virtual void Set<T>(KeyValuePair<string, T>[] values, TimeSpan? expiry = null)
    {
        var caches = new KeyValuePair<RedisKey, RedisValue>[values.Length];
        for (var i = 0; i < values.Length; i++)
            caches[i] = new KeyValuePair<RedisKey, RedisValue>(values[i].Key,
                JsonConvert.SerializeObject(values[i].Value));
        RedisConnection.BasicRetry(db => db.StringSet(caches));
    }

    public virtual Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        return RedisConnection.BasicRetryAsync(async db =>
            await db.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry).ConfigureAwait(false));
    }
}