using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SubhadraSolutions.Utils.Threading;

public delegate void ActionDelegate(object tag);

public sealed class IntervalMultiTask : AbstractDisposable
{
    private static readonly object _syncLock = new();

    private static IntervalMultiTask _defaultInstance;

    private readonly List<ActionInfo> _actions = [];

    private readonly AutoResetEvent _event = new(false);

    private readonly ReaderWriterLockSlim _lock = new();

    private readonly bool _useDedicatedThread;

    private string _invokingThreadName;

    private bool _isStopped;

    private Thread _thread;

    private Timer _timer;

    public IntervalMultiTask(bool useDedicatedThread)
    {
        _useDedicatedThread = useDedicatedThread;
    }

    public static IntervalMultiTask DefaultInstance
    {
        get
        {
            if (_defaultInstance == null)
            {
                lock (_syncLock)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new IntervalMultiTask(false);
                    }
                }
            }

            return _defaultInstance;
        }
    }

    public string InvokingThreadName
    {
        get => _invokingThreadName;

        set
        {
            _invokingThreadName = value;

            if (_thread != null)
            {
                _thread.Name = value;
            }
        }
    }

    ~IntervalMultiTask()
    {
        Dispose(false);
    }

    public void AddAction(ActionDelegate action, long interval, object tag)
    {
        _lock.EnterWriteLock();

        try
        {
            var intervalInTicks = interval * TimeSpan.TicksPerMillisecond;
            _actions.Add(new ActionInfo(action, intervalInTicks, SharedStopwatch.ElapsedTicks, tag));

            if (_actions.Count == 1)
            {
                _isStopped = false;
                Start(interval);
            }
            else
            {
                if (_useDedicatedThread)
                {
                    _event.Set();
                }
                else
                {
                    _timer.Stop();
                    _timer.Interval = 1;
                    _timer.Start();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToDetailedString());
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void RemoveAction(ActionDelegate action, object tag)
    {
        _lock.EnterWriteLock();

        try
        {
            for (var i = 0; i < _actions.Count; i++)
                if (_actions[i].Actiondelegate == action && _actions[i].Tag == tag)
                {
                    _actions.RemoveAt(i);
                    i--;
                }

            if (_actions.Count == 0)
            {
                _isStopped = true;
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (this != _defaultInstance)
        {
            _isStopped = true;

            _lock.Dispose();
        }
    }

    private long PerformActions()
    {
        _lock.EnterReadLock();

        try
        {
            foreach (var v in _actions)
            {
                var diff = SharedStopwatch.ElapsedTicks - v.LastInvokedTime;

                if (diff >= v.IntervalInTicks)
                {
                    try
                    {
                        v.LastInvokedTime = SharedStopwatch.ElapsedTicks;
                        v.Actiondelegate(v.Tag);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToDetailedString());
                    }
                }
            }

            var maxTicksToWait = long.MaxValue;

            for (var i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                var ticks = action.IntervalInTicks - (SharedStopwatch.ElapsedTicks - action.LastInvokedTime);

                if (ticks < maxTicksToWait)
                {
                    maxTicksToWait = ticks;
                }
            }

            return maxTicksToWait <= 0 ? 0 : maxTicksToWait / TimeSpan.TicksPerMillisecond;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private void Start(long sleepInterval)
    {
        if (!_useDedicatedThread)
        {
            _timer = new Timer(sleepInterval);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = false;
            _timer.Start();
        }
        else
        {
            _thread = new Thread(state =>
            {
                try
                {
                    var interval = (long)state;
                    Thread.Sleep((int)interval);

                    while (!_isStopped)
                    {
                        var ticksBefore = SharedStopwatch.ElapsedTicks;
                        var milliSecondsToWait = PerformActions();

                        if (milliSecondsToWait > 0)
                        {
                            _event.WaitOne((int)milliSecondsToWait);
                            _event.Reset();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToDetailedString());
                }
            })
            {
                Name = _invokingThreadName,
                IsBackground = true
            };

            _thread.Start(sleepInterval);
        }
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        Thread.CurrentThread.Name = _invokingThreadName;
        _ = SharedStopwatch.ElapsedTicks;
        var milliSecondsToWait = PerformActions();

        if (!_isStopped)
        {
            _timer.Interval = milliSecondsToWait > 0 ? milliSecondsToWait : 1;
            _timer.Start();
        }
        else
        {
            _timer.Dispose();
        }
    }

    private sealed class ActionInfo(ActionDelegate actiondelegate, long intervalInTicks, long lastInvokedTime, object tag)
    {
        public ActionDelegate Actiondelegate { get; } = actiondelegate;

        public long IntervalInTicks { get; } = intervalInTicks;

        public long LastInvokedTime { get; set; } = lastInvokedTime;

        public object Tag { get; } = tag;
    }
}