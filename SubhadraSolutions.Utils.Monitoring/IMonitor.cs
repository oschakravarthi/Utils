using SubhadraSolutions.Utils.Contracts;
using System;

namespace SubhadraSolutions.Utils.Monitoring;

public interface IMonitor : INamed
{
    AlertStatus AlertStatus { get; }

    public event EventHandler<AlertEventArgs> OnAlertStatusChanged;
}