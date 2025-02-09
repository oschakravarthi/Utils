using System.Collections.Concurrent;

namespace SubhadraSolutions.Utils.Caching.Redis;

/// <summary>
///     Sealed class to avoid instantiating multiple instances of redis connection with same connection string
/// </summary>
public sealed class RedisServiceInstantiator
{
    private static ConcurrentDictionary<string, IRedisService> _redisServices;

    /// <summary>
    ///     returns a redis service instance based on connection string, if the connection is already open with same connection
    ///     string
    ///     then it returns existing connection
    /// </summary>
    public static IRedisService GetRedisService(string connectionString)
    {
        IRedisService service;
        if (_redisServices == null)
        {
            service = new RedisService(connectionString);
            _redisServices = new ConcurrentDictionary<string, IRedisService>();
            _redisServices.TryAdd(connectionString, service);
        }
        else
        {
            if (!_redisServices.TryGetValue(connectionString, out service))
            {
                service = new RedisService(connectionString);
                _redisServices = new ConcurrentDictionary<string, IRedisService>();
                _redisServices.TryAdd(connectionString, service);
            }
        }

        return service;
    }
}