using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Timer = System.Timers.Timer;

namespace SubhadraSolutions.Utils.Threading;

public sealed class IntervalTask(long interval, bool useDedicatedThread, bool strictInterval)
    : AbstractDisposable
{
    private readonly List<KeyValuePair<ActionDelegate, object>> _actions = [];

    private readonly ReaderWriterLockSlim _lock = new();

    private string _invokingThreadName;

    private bool _isStopped = true;

    private Thread _thread;

    private Timer _timer;

    public IntervalTask(long interval, bool useDedicatedThread)
        : this(interval, useDedicatedThread, false)
    {
    }

    public long Interval
    {
        get => interval;

        set
        {
            interval = value;

            if (_timer != null)
            {
                _timer.Interval = value;
            }
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

    public bool IsStarted => !_isStopped;

    ~IntervalTask()
    {
        Dispose(false);
    }

    public void AddAction(ActionDelegate action, object tag)
    {
        _lock.EnterWriteLock();

        try
        {
            _actions.Add(new KeyValuePair<ActionDelegate, object>(action, tag));
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
                if (_actions[i].Key == action && _actions[i].Value == tag)
                {
                    _actions.RemoveAt(i);
                    i--;
                }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Start()
    {
        if (_isStopped)
        {
            _isStopped = false;

            if (!useDedicatedThread)
            {
                _timer = new Timer(Interval);

                _timer.Elapsed += delegate
                {
                    _timer.Stop();
                    Thread.CurrentThread.Name = _invokingThreadName;
                    var ticksBefore = SharedStopwatch.ElapsedTicks;
                    PerformAction();
                    var milliSecondsToWait = interval;

                    if (strictInterval)
                    {
                        milliSecondsToWait = (long)(interval -
                                                    SharedStopwatch.GetElapsed(ticksBefore)
                                                        .TotalMilliseconds);
                        milliSecondsToWait = Math.Max(milliSecondsToWait, 1);
                    }

                    _timer.Interval = milliSecondsToWait;
                    _timer.Start();
                };

                _timer.AutoReset = false;
                _timer.Start();
            }
            else
            {
                _thread = new Thread(() =>
                {
                    Thread.Sleep((int)interval);

                    while (!_isStopped)
                    {
                        var ticksBefore = SharedStopwatch.ElapsedTicks;
                        PerformAction();
                        var milliSecondsToWait = interval;

                        if (strictInterval)
                        {
                            milliSecondsToWait = (long)(interval - TimeSpan
                                .FromTicks(SharedStopwatch.ElapsedTicks - ticksBefore).TotalMilliseconds);
                        }

                        if (milliSecondsToWait > 0)
                        {
                            Thread.Sleep((int)milliSecondsToWait);
                        }
                    }
                })
                {
                    Name = _invokingThreadName,
                    IsBackground = true
                };

                _thread.Start();
            }
        }
    }

    public void Stop()
    {
        _isStopped = true;
        _timer?.Stop();
    }

    protected override void Dispose(bool disposing)
    {
        Stop();
        if (disposing)
        {
            _lock.EnterWriteLock();
        }

        try
        {
            _actions.Clear();
        }
        finally
        {
            if (disposing)
            {
                _lock.ExitWriteLock();
            }
        }

        _lock.Dispose();
    }

    private void PerformAction()
    {
        _lock.EnterReadLock();

        try
        {
            foreach (var kvp in _actions)
            {
                try
                {
                    kvp.Key(kvp.Value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToDetailedString());
                }
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}