using SubhadraSolutions.Utils.SlidingWindows;
using SubhadraSolutions.Utils.SlidingWindows.Contracts;
using System;

namespace SubhadraSolutions.Utils.Monitoring;

public abstract class AbstractMonitor<T> : IMonitor
{
    protected AbstractMonitor(ISlidingWindowCache<T> slidingWindowCache)
    {
        Cache = slidingWindowCache;
        AlertStatus = AlertStatus.Unknown;

        Name = GetType().ToString();

        CheckForAlert();
        Cache.OnDataFetch += Cache_OnDataFetch;
    }

    protected ISlidingWindowCache<T> Cache { get; }

    public event EventHandler<AlertEventArgs> OnAlertStatusChanged;

    public AlertStatus AlertStatus { get; private set; }
    public string Name { get; set; }

    protected abstract AlertCheckResult CheckForAlertCore();

    private void Cache_OnDataFetch(object sender, DataFetchEventArgs<T> e)
    {
        CheckForAlert();
    }

    private void CheckForAlert()
    {
        var previousStatus = AlertStatus;
        var result = CheckForAlertCore();
        var isOn = result.AlertStatus == AlertStatus.On;

        if ((isOn && previousStatus != AlertStatus.On) || (!isOn && previousStatus != AlertStatus.Off))
        {
            AlertStatus = isOn ? AlertStatus.On : AlertStatus.Off;
            FireAlertStatusChanged(result);
        }
    }

    private void FireAlertStatusChanged(AlertCheckResult result)
    {
        var evt = OnAlertStatusChanged;
        if (evt != null)
        {
            var alert = new Alert(result.AlertStatus, Name, result.Message);
            evt(this, new AlertEventArgs(alert));
        }
    }
}