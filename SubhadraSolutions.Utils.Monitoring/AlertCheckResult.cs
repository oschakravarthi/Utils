namespace SubhadraSolutions.Utils.Monitoring;

public class AlertCheckResult(AlertStatus alertStatus, string message = null)
{
    public AlertStatus AlertStatus { get; } = alertStatus;
    public string Message { get; } = message;
}