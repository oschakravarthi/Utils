namespace SubhadraSolutions.Utils.EventProcessing;

public abstract class AbstractEventPublisher
    (IEventAggregator eventAggregator) : AbstractEventingComponent(eventAggregator), IEventPublisher
{
    public string Topic { get; set; } //IaaS.Crashes.Increasing

    protected abstract void PublishEvents();
}