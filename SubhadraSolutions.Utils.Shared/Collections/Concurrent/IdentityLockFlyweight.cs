using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class IdentityLockFlyweight : AbstractInitializable
{
    private readonly Dictionary<Guid, WeakReference> _dictionary = [];

    private readonly ReaderWriterLockSlim _slimLock = new(LockRecursionPolicy.SupportsRecursion);

    private long _cleanupInterval = 60000;

    private IntervalTask _cleanupTask;

    private IdentityLockFlyweight()
    {
    }

    public static IdentityLockFlyweight Instance { get; } = new();

    public long CleanupInterval
    {
        get => _cleanupInterval;
        set
        {
            if (IsInitialized)
            {
                throw new ObjectAlreadyInitializedException();
            }

            if (value < 1)
            {
                throw new ArgumentException("CleanupInterval should be >0");
            }

            _cleanupInterval = value;
        }
    }

    public object GetLock(Guid id)
    {
        if (!IsInitialized)
        {
            Initialize();
        }

        try
        {
            _slimLock.EnterUpgradeableReadLock();
            if (_dictionary.TryGetValue(id, out var reference))
            {
                var target = reference.Target;
                if (target == null)
                {
                    try
                    {
                        _slimLock.EnterWriteLock();
                        target = new object();
                        reference.Target = target;
                    }
                    finally
                    {
                        _slimLock.ExitWriteLock();
                    }
                }

                return target;
            }
            else
            {
                try
                {
                    _slimLock.EnterWriteLock();
                    var target = new object();
                    _dictionary.Add(id, new WeakReference(target));
                    return target;
                }
                finally
                {
                    _slimLock.ExitWriteLock();
                }
            }
        }
        finally
        {
            _slimLock.ExitUpgradeableReadLock();
        }
    }

    protected override void InitializeProtected()
    {
        _cleanupTask = new IntervalTask(CleanupInterval, false);
        _cleanupTask.AddAction(CleanupDictionary, null);
        _cleanupTask.Start();
    }

    private void CleanupDictionary(object tag)
    {
        try
        {
            _cleanupTask.Stop();
            _slimLock.EnterWriteLock();
            var list = new List<Guid>();
            foreach (var kvp in _dictionary)
            {
                var target = kvp.Value.Target;
                if (target == null)
                {
                    list.Add(kvp.Key);
                }
            }

            for (var i = 0; i < list.Count; i++) _dictionary.Remove(list[i]);
        }
        finally
        {
            _slimLock.ExitWriteLock();
            _cleanupTask.Start();
        }
    }
}