using SubhadraSolutions.Utils.Threading;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public delegate TValue CreateObjectCallback<out TValue, in TKey>(TKey key);

public sealed class CompositeDictionary<TKey, TValue>
    (bool shouldBeThreadSafe) : AbstractReaderWriterAtomicOperationSupported(shouldBeThreadSafe)
{
    private readonly TKey keyInParent;

    private readonly CompositeDictionary<TKey, TValue> parent;

    private Dictionary<TKey, TValue> dictionary;

    private Dictionary<TKey, CompositeDictionary<TKey, TValue>> innerDictionaries;

    private CompositeDictionary(bool shouldThreadSafe, CompositeDictionary<TKey, TValue> parent, TKey keyInParent)
        : this(shouldThreadSafe)
    {
        this.parent = parent;
        this.keyInParent = keyInParent;
    }

    public void AddOrUpdate(TValue value, params TKey[] keys)
    {
        AddOrUpdate(0, value, keys);
    }

    public void Clear()
    {
        LockSlim?.EnterWriteLock();

        try
        {
            innerDictionaries = null;
            dictionary = null;
        }
        finally
        {
            LockSlim?.ExitWriteLock();
        }
    }

    public TValue Get(params TKey[] keys)
    {
        TryGetValue(out var value, keys);
        return value;
    }

    public List<CompositeKeyValue<TKey, TValue>> GetCompleteTree()
    {
        return GetCompleteTree(null, true);
    }

    public List<CompositeKeyValue<TKey, TValue>> GetCompleteTree(IComparer<CompositeKeyValue<TKey, TValue>> comparer,
        bool assending)
    {
        LockSlim?.EnterReadLock();

        try
        {
            var compositeKeyValuesDictionary = new Dictionary<TKey, CompositeKeyValue<TKey, TValue>>();
            if (dictionary != null)
            {
                foreach (var kvp in dictionary)
                {
                    var composite = new CompositeKeyValue<TKey, TValue>(kvp);
                    compositeKeyValuesDictionary.Add(kvp.Key, composite);
                }
            }

            if (innerDictionaries != null)
            {
                foreach (var kvp in innerDictionaries)
                {
                    if (!compositeKeyValuesDictionary.TryGetValue(kvp.Key, out var composite))
                    {
                        composite = new CompositeKeyValue<TKey, TValue>(kvp.Key, default);
                        compositeKeyValuesDictionary.Add(kvp.Key, composite);
                    }

                    composite.InnerKeyValues = kvp.Value.GetCompleteTree(comparer, assending);
                    foreach (var compositeKeyValue in composite.InnerKeyValues)
                    {
                        compositeKeyValue.Parent = composite;
                    }
                }
            }

            var result = compositeKeyValuesDictionary.Values.ToList();
            if (comparer != null)
            {
                if (!assending)
                {
                    comparer = new InverseComparer<CompositeKeyValue<TKey, TValue>>(comparer);
                }

                result.Sort(comparer);
            }

            return result;
        }
        finally
        {
            LockSlim?.ExitReadLock();
        }
    }

    public List<TKey> GetKeys(params TKey[] keys)
    {
        return getKeys(0, keys);
    }

    public TValue GetOrAddAndGet(CreateObjectCallback<TValue, TKey> callback, params TKey[] keys)
    {
        return getOrAddAndGet(0, callback, keys);
    }

    public bool RemoveIfExists(params TKey[] keys)
    {
        return removeIfExists(0, keys);
    }

    public bool TryGetValue(out TValue value, params TKey[] keys)
    {
        return tryGetvalue(0, out value, keys);
    }

    protected override void Dispose(bool disposing)
    {
        if (LockSlim != null)
        {
            if (disposing)
            {
                LockSlim.EnterReadLock();
            }

            try
            {
                if (innerDictionaries != null)
                {
                    foreach (var kvp in innerDictionaries)
                    {
                        kvp.Value.Dispose();
                    }
                }
            }
            finally
            {
                if (disposing)
                {
                    LockSlim.ExitReadLock();
                }
            }

            LockSlim.Dispose();
        }
    }

    private void AddOrUpdate(int startIndex, TValue value, params TKey[] keys)
    {
        if (keys.Length - 1 == startIndex)
        {
            LockSlim?.EnterWriteLock();

            try
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<TKey, TValue> { { keys[startIndex], value } };
                }
                else
                {
                    dictionary[keys[startIndex]] = value;
                }
            }
            finally
            {
                LockSlim?.ExitWriteLock();
            }
        }
        else
        {
            var cd = getOrCreateGetCompositeDictionary(startIndex, keys);
            cd.AddOrUpdate(startIndex + 1, value, keys);
        }
    }

    private IEnumerable<TKey> getInnerKeys()
    {
        if (innerDictionaries != null)
        {
            return innerDictionaries.Keys;
        }

        return null;
    }

    private List<TKey> getKeys()
    {
        var list = new List<TKey>();
        if (dictionary != null)
        {
            list.AddRange(dictionary.Keys);
        }

        var innerList = getInnerKeys();
        if (innerList != null)
        {
            list.AddRange(innerList);
        }

        return list;
    }

    private List<TKey> getKeys(int startIndex, IList<TKey> keys)
    {
        if (keys == null || keys.Count == startIndex)
        {
            if (LockSlim == null)
            {
                return getKeys();
            }

            LockSlim.EnterReadLock();
            try
            {
                return getKeys();
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }

        LockSlim?.EnterReadLock();

        try
        {
            if (innerDictionaries == null)
            {
                return [];
            }

            if (innerDictionaries.TryGetValue(keys[startIndex], out var cd))
            {
                return cd.getKeys(startIndex + 1, keys);
            }

            return [];
        }
        finally
        {
            LockSlim?.ExitReadLock();
        }
    }

    private TValue getOrAddAndGet(int startIndex, CreateObjectCallback<TValue, TKey> callback, params TKey[] keys)
    {
        if (keys.Length - 1 == startIndex)
        {
            LockSlim?.EnterWriteLock();

            try
            {
                TValue value;
                if (dictionary == null)
                {
                    dictionary = [];
                    value = callback(keys[startIndex]);
                    dictionary.Add(keys[startIndex], value);
                    return value;
                }

                if (!dictionary.TryGetValue(keys[startIndex], out value))
                {
                    value = callback(keys[startIndex]);
                    dictionary.Add(keys[startIndex], value);
                }

                return value;
            }
            finally
            {
                LockSlim?.ExitWriteLock();
            }
        }

        var cd = getOrCreateGetCompositeDictionary(startIndex, keys);
        return cd.getOrAddAndGet(startIndex + 1, callback, keys);
    }

    private CompositeDictionary<TKey, TValue> getOrCreateGetCompositeDictionary(int startIndex, params TKey[] keys)
    {
        LockSlim?.EnterWriteLock();

        try
        {
            CompositeDictionary<TKey, TValue> cd;
            if (innerDictionaries == null)
            {
                innerDictionaries = [];
                cd = new CompositeDictionary<TKey, TValue>(LockSlim != null, this, keys[startIndex]);
                innerDictionaries.Add(keys[startIndex], cd);
            }
            else
            {
                if (!innerDictionaries.TryGetValue(keys[startIndex], out cd))
                {
                    cd = new CompositeDictionary<TKey, TValue>(LockSlim != null, this, keys[startIndex]);
                    innerDictionaries.Add(keys[startIndex], cd);
                }
            }

            return cd;
        }
        finally
        {
            LockSlim?.ExitWriteLock();
        }
    }

    private void OnInnerEmptied(CompositeDictionary<TKey, TValue> innerDictionary)
    {
        LockSlim?.EnterWriteLock();

        try
        {
            innerDictionaries.Remove(innerDictionary.keyInParent);
            if (innerDictionaries.Count == 0)
            {
                innerDictionaries = null;
                if (parent != null && dictionary == null)
                {
                    parent.OnInnerEmptied(this);
                }
            }
        }
        finally
        {
            LockSlim?.ExitWriteLock();
        }
    }

    private bool removeIfExists(int startIndex, params TKey[] keys)
    {
        if (keys.Length - 1 == startIndex)
        {
            LockSlim?.EnterWriteLock();

            try
            {
                if (dictionary != null)
                {
                    if (dictionary.Remove(keys[startIndex]))
                    {
                        if (dictionary.Count == 0)
                        {
                            dictionary = null;
                            if (parent != null && innerDictionaries == null)
                            {
                                parent.OnInnerEmptied(this);
                            }
                        }

                        return true;
                    }
                }

                return false;
            }
            finally
            {
                LockSlim?.ExitWriteLock();
            }
        }

        LockSlim?.EnterUpgradeableReadLock();

        try
        {
            if (innerDictionaries != null)
            {
                if (innerDictionaries.TryGetValue(keys[startIndex], out var cd))
                {
                    return cd.removeIfExists(startIndex + 1, keys);
                }
            }

            return false;
        }
        finally
        {
            LockSlim?.ExitUpgradeableReadLock();
        }
    }

    private bool tryGetvalue(int startIndex, out TValue value, params TKey[] keys)
    {
        if (keys.Length - 1 == startIndex)
        {
            LockSlim?.EnterReadLock();

            try
            {
                if (dictionary != null)
                {
                    return dictionary.TryGetValue(keys[startIndex], out value);
                }

                value = default;
                return false;
            }
            finally
            {
                LockSlim?.ExitReadLock();
            }
        }

        LockSlim?.EnterReadLock();

        try
        {
            if (innerDictionaries == null)
            {
                value = default;
                return false;
            }
            else
            {
                if (innerDictionaries.TryGetValue(keys[startIndex], out var cd))
                {
                    return cd.tryGetvalue(startIndex + 1, out value, keys);
                }

                value = default;
                return false;
            }
        }
        finally
        {
            LockSlim?.ExitReadLock();
        }
    }
}