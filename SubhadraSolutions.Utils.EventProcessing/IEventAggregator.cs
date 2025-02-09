using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.EventProcessing;

public interface IEventAggregator
{
    IEnumerable<IEvent> GetSourceChain(Guid eventId);

    IEnumerable<IEvent> GetUnhandledEvents();

    void PublishEvent(IEvent eventObject);

    IDisposable Subscribe(string topicWildcard, EventHandler<IEvent> notificationCallback);

    IDisposable Subscribe(EventHandler<IEvent> notificationCallback, params string[] topicWildcards);
}