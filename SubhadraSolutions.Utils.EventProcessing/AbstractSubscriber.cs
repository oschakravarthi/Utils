using System;

namespace SubhadraSolutions.Utils.EventProcessing;

public abstract class AbstractSubscriber(IEventAggregator eventAggregator) : AbstractEventingComponent(eventAggregator),
    IEventSubscriber
{
    private IDisposable unsubscriber;

    public string[] Topics { get; set; }

    protected override void Dispose(bool disposing)
    {
        unsubscriber.Dispose();
    }

    protected override void InitializeProtected()
    {
        unsubscriber = eventAggregator.Subscribe(OnEvent, Topics);
    }

    protected abstract void OnEvent(object sender, IEvent e);
}