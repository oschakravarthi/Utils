using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Collections.Concurrent;
using SubhadraSolutions.Utils.Text.RegularExpressions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SubhadraSolutions.Utils.EventProcessing;

public sealed class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Guid, IEvent> allEvents;

    private readonly IList<Tuple<string, EventHandler<IEvent>, Regex>> subscriberCallbacks;

    private readonly IList<IEvent> unhandledEvents;

    public EventAggregator(bool storeAllEvents)
    {
        subscriberCallbacks = [];
        subscriberCallbacks = new SafeListDecorator<Tuple<string, EventHandler<IEvent>, Regex>>(subscriberCallbacks);

        unhandledEvents = [];
        unhandledEvents = new SafeListDecorator<IEvent>(unhandledEvents);

        if (storeAllEvents)
        {
            allEvents = new ConcurrentDictionary<Guid, IEvent>();
        }
    }

    public IEnumerable<IEvent> GetSourceChain(Guid eventId)
    {
        if (allEvents != null)
        {
            Guid? temp = eventId;
            while (temp != null)
                if (allEvents.TryGetValue(temp.Value, out var evt))
                {
                    temp = evt.SourceEventId;
                    yield return evt;
                }
        }
    }

    public IEnumerable<IEvent> GetUnhandledEvents()
    {
        return unhandledEvents;
    }

    public void PublishEvent(IEvent eventObject)
    {
        //Guard.ArgumentShouldNotBeNull(eventObject, nameof(eventObject));
        allEvents?.TryAdd(eventObject.Id, eventObject);

        NotifySubscribers(eventObject);
    }

    public IDisposable Subscribe(string topicWildcard, EventHandler<IEvent> notificationCallback)
    {
        //Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(topicWildcard, nameof(topicWildcard));
        //Guard.ArgumentShouldNotBeNull(notificationCallback, nameof(notificationCallback));
        var regex = RegexHelper.BuildRegexFromWildcard(topicWildcard, true);
        var tuple = new Tuple<string, EventHandler<IEvent>, Regex>(topicWildcard, notificationCallback,
            regex);

        subscriberCallbacks.Add(tuple);
        return new Unsubscribe(tuple, subscriberCallbacks);
    }

    public IDisposable Subscribe(EventHandler<IEvent> notificationCallback, params string[] topicWildcards)
    {
        //Guard.ArgumentShouldNotBeNull(topicWildcards, nameof(topicWildcards));
        //foreach (var topicWildcard in topicWildcards)
        //{
        //    Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(topicWildcard, nameof(topicWildcard));
        //}

        var unsubscribeMiltiple = new UnsubscribeMiltiple();
        foreach (var topicWildcard in topicWildcards)
        {
            var disposable = Subscribe(topicWildcard, notificationCallback);
            unsubscribeMiltiple.Disposables.Add(disposable);
        }

        return unsubscribeMiltiple;
    }

    private void NotifySubscribers(IEvent eventObject)
    {
        var foundSubscriber = false;
        foreach (var tuple in subscriberCallbacks)
        {
            if (tuple.Item3.IsMatch(eventObject.Topic))
            {
                foundSubscriber = true;
                tuple.Item2(this, eventObject);
            }
        }

        if (!foundSubscriber)
        {
            unhandledEvents.Add(eventObject);
        }
    }

    private sealed class Unsubscribe : AbstractDisposable
    {
        private readonly IList<Tuple<string, EventHandler<IEvent>, Regex>> _list;
        private readonly Tuple<string, EventHandler<IEvent>, Regex> tuple;

        internal Unsubscribe(Tuple<string, EventHandler<IEvent>, Regex> tuple,
            IList<Tuple<string, EventHandler<IEvent>, Regex>> list)
        {
            this.tuple = tuple;
            _list = list;
        }

        protected override void Dispose(bool disposing)
        {
            _list.Remove(tuple);
        }
    }

    private sealed class UnsubscribeMiltiple : AbstractDisposable
    {
        public List<IDisposable> Disposables { get; } = [];

        protected override void Dispose(bool disposing)
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
        }
    }
}