using System;

namespace SubhadraSolutions.Utils.Monitoring;

public class Alert(AlertStatus status, string monitorName, string message)
{
    public string Message { get; } = message;
    public string MonitorName { get; } = monitorName;
    public AlertStatus Status { get; } = status;
    public DateTime Timestamp { get; } = GlobalSettings.Instance.DateTimeNow;
}