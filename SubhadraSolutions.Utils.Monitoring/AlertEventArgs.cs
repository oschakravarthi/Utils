using System;

namespace SubhadraSolutions.Utils.Monitoring;

public class AlertEventArgs(Alert alert) : EventArgs
{
    public Alert Alert { get; } = alert;
}