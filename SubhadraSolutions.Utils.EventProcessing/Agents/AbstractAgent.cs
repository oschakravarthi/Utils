namespace SubhadraSolutions.Utils.EventProcessing.Agents;

public abstract class AbstractAgent(IEventAggregator eventAggregator) : AbstractSubscriber(eventAggregator), IEventAgent
{
    public string Topic { get; set; }
}