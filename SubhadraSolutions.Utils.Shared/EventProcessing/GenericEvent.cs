using System;

namespace SubhadraSolutions.Utils.EventProcessing;

public class GenericEvent<T>(string topic, T payload, Guid? sourceEventId = null) : IEvent
{
    public T Payload { get; } = payload;

    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccuredOn { get; } = GlobalSettings.Instance.DateTimeNow;

    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    public object PayloadObject => Payload;

    public Guid? SourceEventId { get; } = sourceEventId;
    public string Topic { get; } = topic;
}