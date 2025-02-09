using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Caching.Redis;

public interface IRedisService
{
    /// <summary>
    ///     Removes the specified key. A key is ignored if it does not exist
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <returns>True if the key was removed</returns>
    bool Delete(string key);

    /// <summary>
    ///     Removes the specified key. A key is ignored if it does not exist
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <returns>True if the key was removed</returns>
    Task<bool> DeleteAsync(string key);

    /// <summary>
    ///     Removes all the keys starting with the keyPrefix
    /// </summary>
    /// <param name="keyPrefix"></param>
    /// <returns>The number of keys that were removed</returns>
    long DeleteKeysByPrefix(string keyPrefix);

    /// <summary>
    ///     Returns the values of a specified keys. If a key does not exist, the null is returned.
    /// </summary>
    /// <returns>The values of the strings with null for keys do not exist</returns>
    T Get<T>(string key);

    /// <summary>
    ///     Returns the values of all specified keys. For every key that does not hold a string value or does not exist, the
    ///     special value null is returned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys"></param>
    /// <returns></returns>
    IEnumerable<T> Get<T>(IEnumerable<string> keys);

    /// <summary>
    ///     Returns the values of all specified keys. For every key that does not hold a
    ///     string value or does not exist, the special value null is returned.
    /// </summary>
    /// <returns>The values of the strings with null for keys do not exist</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    ///     Returns the values of all specified keys. For every key that does not hold a string value or does not exist, the
    ///     special value null is returned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAsync<T>(IEnumerable<string> keys);

    /// <summary>
    ///     Set key to hold the string value. If key already holds a value, it is overwritten,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key of the string</param>
    /// <param name="value">The value to set</param>
    /// <param name="expiry">The expiry to set</param>
    void Set<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    ///     Sets the given keys to their respective values. If "not exists" is specified,
    ///     this will not perform any operation at all even if just a single key already exists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key of the string</param>
    /// <param name="value">The value to set</param>
    /// <param name="expiry">The expiry to set</param>
    void Set<T>(KeyValuePair<string, T>[] values, TimeSpan? expiry = null);

    /// <summary>
    ///     Set key to hold the string value. If key already holds a value, it is overwritten,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key of the string</param>
    /// <param name="value">The value to set</param>
    /// <param name="expiry">The expiry to set</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
}