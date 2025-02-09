using SubhadraSolutions.Utils.Validation;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Generic;

public static class GlobalDictionary
{
    private static readonly ReaderWriterLockSlim _slim = new(LockRecursionPolicy.SupportsRecursion);
    private static readonly Dictionary<string, object> _dictionary = new(StringComparer.CurrentCultureIgnoreCase);

    public static void AddOrUpdate(string key, object value)
    {
        Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(key, nameof(key));
        try
        {
            _slim.EnterWriteLock();
            _dictionary[key] = value;
        }
        finally
        {
            _slim.ExitWriteLock();
        }
    }

    public static object Get(string key)
    {
        Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(key, nameof(key));
        try
        {
            _slim.EnterReadLock();
            _dictionary.TryGetValue(key, out var value);
            return value;
        }
        finally
        {
            _slim.ExitReadLock();
        }
    }
}