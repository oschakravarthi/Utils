namespace SubhadraSolutions.Utils.EventProcessing.Agents;

public class ReplayAgent(IEventAggregator eventAggregator) : AbstractAgent(eventAggregator)
{
    protected override void OnEvent(object sender, IEvent e)
    {
        EventProcessingHelper.BuildGenericEvent(Topic, e.Topic, e.Id);
    }
}