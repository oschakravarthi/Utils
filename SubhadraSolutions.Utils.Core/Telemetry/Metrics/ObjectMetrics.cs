namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public class ObjectMetrics
{
    public ObjectMetrics(string objectType, string objectName, Metric[] metrics, bool isObjectGarbageCollected)
    {
        ObjectType = objectType;
        ObjectName = objectName;
        Metrics = metrics;
        IsObjectGarbageCollected = isObjectGarbageCollected;
    }

    public bool IsObjectGarbageCollected { get; }
    public Metric[] Metrics { get; }
    public string ObjectName { get; }
    public string ObjectType { get; }
}