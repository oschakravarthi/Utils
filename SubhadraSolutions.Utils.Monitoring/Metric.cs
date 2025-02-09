using System;

namespace SubhadraSolutions.Utils.Monitoring;

public class Metric(DateTime timestamp, double value)
{
    public DateTime Timestamp { get; } = timestamp;
    public double Value { get; } = value;
}