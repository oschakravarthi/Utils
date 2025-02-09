using System;

namespace SubhadraSolutions.Utils.EventProcessing;

public static class EventProcessingHelper
{
    public static IEvent BuildGenericEvent(string topic, object payload, Guid? sourceEventId = null)
    {
        var payloadType = payload.GetType();
        return (IEvent)Activator.CreateInstance(typeof(GenericEvent<>).MakeGenericType(payloadType), topic, payload,
            sourceEventId);
    }

    public static IEvent GetSourceEventByTopic(this IEventAggregator eventAggregator, Guid eventId, string topic)
    {
        foreach (var e in eventAggregator.GetSourceChain(eventId))
        {
            if (e.Topic == topic)
            {
                return e;
            }
        }

        return null;
    }
}