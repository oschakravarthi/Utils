using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SubhadraSolutions.Utils.Threading;

public sealed class ScheduledTask : AbstractDisposable
{
    private readonly bool _performTaskOnStart;

    private readonly object _syncLockForStart = new();

    private readonly Timer _timer;

    private readonly TimeOnly[] _timings;

    private int _index = -1;

    private volatile bool _isStarted;

    private DateTime _nextRunOn;

    public ScheduledTask(TaskScheduleSettings settings)
        : this(settings.Timings, settings.PerformTaskOnStart)
    {
    }

    public ScheduledTask(IEnumerable<TimeOnly> timings, bool performTaskOnStart)
        : this(performTaskOnStart, timings.ToArray())
    {
    }

    public ScheduledTask(bool performTaskOnStart, params TimeOnly[] timings)
    {
        if (timings.Length == 0)
        {
            throw new ArgumentException("Timings should not be empty", nameof(timings));
        }

        _performTaskOnStart = performTaskOnStart;
        var hashSet = new HashSet<TimeOnly>();

        foreach (var time in timings)
        {
            hashSet.Add(time);
        }

        var temp = hashSet.ToList();
        temp.Sort(TimeOnlyComparer.Instance);
        _timings = temp.ToArray();
        _timer = new Timer();
        _timer.Elapsed += TimerElapsed;
        _timer.Enabled = false;
    }

    public bool IsStarted
    {
        get => _isStarted;

        private set => _isStarted = value;
    }

    public event EventHandler OnSchedule;

    public void Start()
    {
        if (!IsStarted)
        {
            lock (_syncLockForStart)
            {
                if (!IsStarted)
                {
                    IsStarted = true;

                    if (_performTaskOnStart)
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            RaiseOnSchedule();
                            StartCore();
                        });
                    }
                    else
                    {
                        StartCore();
                    }
                }
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        GeneralHelper.DisposeIfDisposable(_timer);
    }

    private long DetermineNextRunOn()
    {
        var daysToAdd = 0;
        var currentDate = GlobalSettings.Instance.DateTimeNow;
        var currentTime = TimeOnly.FromDateTime(currentDate);

        if (_index == _timings.Length - 1)
        {
            _index = 0;
            daysToAdd = 1;
        }
        else
        {
            _index = (_index + 1) % _timings.Length;

            for (; _index < _timings.Length; _index++)
                if (currentTime <= _timings[_index])
                {
                    break;
                }

            if (_index == _timings.Length)
            {
                _index = 0;
            }
        }

        var t = _timings[_index];
        _nextRunOn = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, t.Hour, t.Minute, t.Second,
            t.Millisecond);
        _nextRunOn = _nextRunOn.AddDays(daysToAdd);
        var waitTime = (long)(_nextRunOn - GlobalSettings.Instance.DateTimeNow).TotalMilliseconds;
        return Math.Max(waitTime, 1);
    }

    private void RaiseOnSchedule()
    {
        OnSchedule?.Invoke(this, EventArgs.Empty);
    }

    private void StartCore()
    {
        _timer.Interval = DetermineNextRunOn();
        _timer.Enabled = true;
        _timer.Start();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        try
        {
            Debug.WriteLine("raising event on: " +
                            GlobalSettings.Instance.DateTimeNow.ToString(CultureInfo.InvariantCulture));
            RaiseOnSchedule();
        }
        finally
        {
            _timer.Interval = DetermineNextRunOn();
            _timer.Start();
        }
    }
}