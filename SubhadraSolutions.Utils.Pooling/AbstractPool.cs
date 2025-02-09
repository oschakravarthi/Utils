using SubhadraSolutions.Utils.Pooling.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Pooling;

public delegate void ReturnAdaptedObjectToPoolDelegate<AT>(IPoolItem<AT> poolItem, AT adaptedObject);

public abstract class AbstractPool<TAdaptedObject, TPoolItem>
    where TPoolItem : IPoolItem<TAdaptedObject>, new()
{
    public delegate bool CanSelectThisAdaptedObjectDelegate(TAdaptedObject adaptedObject,
        bool isNewlyCreatedAdaptedObject, object tag);

    private static readonly bool _isAdaptedObjectDisposableType =
        typeof(IDisposable).IsAssignableFrom(typeof(TAdaptedObject));

    private static int _poolIdSequencer;

    private readonly AutoResetEvent _clearingThreadWakeupWaitHandle = new(false);

    private readonly AutoResetEvent _objectReturnedToPoolWaitHandle = new(false);

    private readonly int _poolId;

    private readonly Dictionary<TAdaptedObject, PoolItemInfo> _poolItemsDictionary = [];

    private readonly ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.SupportsRecursion);

    private Thread _clearingThread;

    private Semaphore _countControllingSemaphore;

    private WaitHandle[] _getObjectWaitHandles;

    private int _maxPoolSize = int.MaxValue;

    private int _objectMaxIdleTimeInMilliseconds = int.MaxValue;

    protected AbstractPool(int maxPoolSize, int objectMaxIdleTimeInMilliseconds, bool canObjectBeShared)
        : this()
    {
        MaxPoolSize = maxPoolSize;
        ObjectMaxIdleTimeInMilliseconds = objectMaxIdleTimeInMilliseconds;
        CanObjectBeShared = canObjectBeShared;
    }

    protected AbstractPool()
    {
        _poolId = Interlocked.Increment(ref _poolIdSequencer);
    }

    public bool CanObjectBeShared { get; set; }

    public int Count
    {
        get
        {
            try
            {
                _rwLock.EnterReadLock();
                return _poolItemsDictionary.Count;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }
    }

    public bool IsInitialized { get; private set; }

    public int MaxPoolSize
    {
        get => _maxPoolSize;

        set
        {
            if (value != _maxPoolSize)
            {
                if (IsInitialized)
                {
                    throw new ObjectAlreadyInitializedException(
                        "MaxPoolSize cannot be changed after the Pool is initialized");
                }

                if (value <= 0)
                {
                    throw new ArgumentException(@"MaxPoolSize should be greater than zero", nameof(MaxPoolSize));
                }

                _maxPoolSize = value;
            }
        }
    }

    public int ObjectMaxIdleTimeInMilliseconds
    {
        get => _objectMaxIdleTimeInMilliseconds;

        set
        {
            if (value != _objectMaxIdleTimeInMilliseconds)
            {
                if (value <= 0)
                {
                    throw new ArgumentException(@"ObjectMaxIdleTimeInMilliseconds should be greater than zero",
                        nameof(ObjectMaxIdleTimeInMilliseconds));
                }

                _objectMaxIdleTimeInMilliseconds = value;
                _clearingThreadWakeupWaitHandle.Set();
            }
        }
    }

    protected IAsyncResult BeginGetObject(CanSelectThisAdaptedObjectDelegate callback, object tag = null)
    {
        var result = new PoolAsyncResult(_poolId, GetType());
        Task.Factory.StartNew(delegate
        {
            var obj = GetObject(callback, tag);
            result.SetCompleted(obj);
        });
        return result;
    }

    protected IAsyncResult BeginTryGetObject(CanSelectThisAdaptedObjectDelegate callback, int millisecondsToWait,
        object tag = null)
    {
        var result = new PoolAsyncResult(_poolId, GetType());
        Task.Factory.StartNew(() =>
        {
            var hasGot = TryGetObject(out var obj, callback, millisecondsToWait, tag);
            result.SetCompleted(new KeyValuePair<bool, TPoolItem>(hasGot, obj));
        });
        return result;
    }

    protected virtual bool CanSelectThisAdaptedObject(TAdaptedObject adaptedObject, bool isNewlyCreatedAdaptedObject,
        object tag)
    {
        return true;
    }

    protected virtual void CleanupAdaptedObject(TAdaptedObject adaptedObject)
    {
        if (_isAdaptedObjectDisposableType)
        {
            ((IDisposable)adaptedObject).Dispose();
        }
    }

    protected TPoolItem EndGetObjectCore(IAsyncResult result)
    {
        ValidateAsyncResult(result);
        result.AsyncWaitHandle.WaitOne();
        return (TPoolItem)result.AsyncState;
    }

    protected bool EndTryGetObjectCore(IAsyncResult result, out TPoolItem obj)
    {
        ValidateAsyncResult(result);
        result.AsyncWaitHandle.WaitOne();
        var kvp = (KeyValuePair<bool, TPoolItem>)result.AsyncState;
        obj = kvp.Value;
        return kvp.Key;
    }

    protected abstract TAdaptedObject GetAdaptedObject();

    protected TPoolItem GetObject(CanSelectThisAdaptedObjectDelegate callback, object tag = null)
    {
        if (callback == null)
        {
            callback = CanSelectThisAdaptedObject;
        }

        CheckAndInitialize();
        TPoolItem adaptedObject;
        while (!TryGetObjectPrivate(out adaptedObject, callback, 100, tag))
        {
        }

        return adaptedObject;
    }

    protected bool TryGetObject(out TPoolItem adaptedObject, CanSelectThisAdaptedObjectDelegate callback,
        int millisecondsToWait, object tag = null)
    {
        if (millisecondsToWait < 0)
        {
            throw new ArgumentException(@"The argument value should be greater than -1", nameof(millisecondsToWait));
        }

        if (callback == null)
        {
            callback = CanSelectThisAdaptedObject;
        }

        CheckAndInitialize();
        return TryGetObjectPrivate(out adaptedObject, callback, millisecondsToWait, tag);
    }

    private void CheckAndInitialize()
    {
        if (!IsInitialized)
        {
            lock (_poolItemsDictionary)
            {
                if (!IsInitialized)
                {
                    IsInitialized = true;
                    _countControllingSemaphore = new Semaphore(MaxPoolSize, MaxPoolSize);
                    _getObjectWaitHandles = [_countControllingSemaphore, _objectReturnedToPoolWaitHandle];
                    _clearingThread = new Thread(ClearingThreadStart) { IsBackground = true };
                    _clearingThread.Start();
                }
            }
        }
    }

    private void CleanupAdaptedObjects(IEnumerable<TAdaptedObject> adaptedObjects)
    {
        foreach (var adaptedObject in adaptedObjects)
        {
            try
            {
                CleanupAdaptedObject(adaptedObject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToDetailedString());
            }
        }
    }

    private void ClearingThreadStart()
    {
        try
        {
            while (true)
            {
                var startedDateTime = DateTime.Now;
                var objectMaxLifeTime = TimeSpan.FromMilliseconds(ObjectMaxIdleTimeInMilliseconds);
                var minTimeSpan = objectMaxLifeTime;
                var maximumPoolSize = MaxPoolSize;
                var adaptedObjectsToCleanup = new List<TAdaptedObject>();
                try
                {
                    _rwLock.EnterUpgradeableReadLock();
                    var i = 0;
                    foreach (var kvp in _poolItemsDictionary)
                    {
                        var info = kvp.Value;
                        if (Interlocked.Read(ref info.usageCount) == 0)
                        {
                            var objectRemainingLifeTime = objectMaxLifeTime - (startedDateTime - info.lastUsedOn);
                            if (objectRemainingLifeTime < TimeSpan.Zero || i >= maximumPoolSize)
                            {
                                adaptedObjectsToCleanup.Add(kvp.Key);
                            }
                            else
                            {
                                if (objectRemainingLifeTime < minTimeSpan)
                                {
                                    minTimeSpan = objectRemainingLifeTime;
                                }
                            }
                        }

                        i++;
                    }

                    if (adaptedObjectsToCleanup.Count > 0)
                    {
                        try
                        {
                            i = 0;
                            _rwLock.EnterWriteLock();
                            for (; i < adaptedObjectsToCleanup.Count; i++)
                            {
                                _poolItemsDictionary.Remove(adaptedObjectsToCleanup[i]);
                                _countControllingSemaphore.Release();
                            }
                        }
                        finally
                        {
                            _rwLock.ExitWriteLock();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToDetailedString());
                }
                finally
                {
                    _rwLock.ExitUpgradeableReadLock();
                    CleanupAdaptedObjects(adaptedObjectsToCleanup);
                }

                var timeToSleep = (int)(minTimeSpan - (DateTime.Now - startedDateTime)).TotalMilliseconds;
                _clearingThreadWakeupWaitHandle.WaitOne(Math.Max(timeToSleep, 1));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToDetailedString());
        }
    }

    private void ReturnObjectToPool(IPoolItem<TAdaptedObject> poolItem, TAdaptedObject adaptedObject)
    {
        try
        {
            _rwLock.EnterReadLock();
            if (_poolItemsDictionary.TryGetValue(adaptedObject, out var poolItemInfo))
            {
                if (Interlocked.Decrement(ref poolItemInfo.usageCount) == 0)
                {
                    poolItemInfo.lastUsedOn = DateTime.Now;
                    poolItem.SetAdaptedObjectAndCallback(default, ReturnObjectToPool);
                    _objectReturnedToPoolWaitHandle.Set();
                }
            }
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    private bool TryGetObjectPrivate(out TPoolItem adaptedObject, CanSelectThisAdaptedObjectDelegate callback,
        int millisecondsToWait, object tag)
    {
        adaptedObject = default;
        var startedTime = DateTime.Now;
        do
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                PoolItemInfo selectedItem = null;
                TAdaptedObject selectedAdaptedObject = default;
                foreach (var kvp in _poolItemsDictionary)
                {
                    if (CanObjectBeShared)
                    {
                        if (callback(kvp.Key, false, tag))
                        {
                            selectedAdaptedObject = kvp.Key;
                            selectedItem = kvp.Value;
                            break;
                        }
                    }
                    else
                    {
                        if (Interlocked.Read(ref kvp.Value.usageCount) == 0)
                        {
                            if (callback(kvp.Key, false, tag))
                            {
                                selectedAdaptedObject = kvp.Key;
                                selectedItem = kvp.Value;
                                break;
                            }
                        }
                    }
                }

                if (selectedItem == null)
                {
                    _objectReturnedToPoolWaitHandle.Reset();
                    var index = WaitHandle.WaitAny(_getObjectWaitHandles, millisecondsToWait);

                    if (index < 0)
                    {
                        return false;
                    }

                    if (index == 0)
                    {
                        try
                        {
                            _rwLock.EnterWriteLock();
                            var a = GetAdaptedObject();
                            callback(a, true, tag);
                            selectedItem = new PoolItemInfo();
                            selectedAdaptedObject = a;
                            _poolItemsDictionary.Add(selectedAdaptedObject, selectedItem);
                        }
                        finally
                        {
                            _rwLock.ExitWriteLock();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                Interlocked.Increment(ref selectedItem.usageCount);
                var pt = new TPoolItem();
                pt.SetAdaptedObjectAndCallback(selectedAdaptedObject, ReturnObjectToPool);
                adaptedObject = pt;
                return true;
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
                millisecondsToWait -= (int)(DateTime.Now - startedTime).TotalMilliseconds;
            }
        } while (millisecondsToWait > 0);

        return false;
    }

    private void ValidateAsyncResult(IAsyncResult result)
    {
        if (!(result is PoolAsyncResult pResult) || pResult.PoolId != _poolId || pResult.PoolType != GetType())
        {
            throw new ArgumentException(@"The IAsyncResult that is passed is not created by this pool", nameof(result));
        }
    }

    private sealed class PoolItemInfo
    {
        internal DateTime lastUsedOn;

        internal long usageCount;
    }
}