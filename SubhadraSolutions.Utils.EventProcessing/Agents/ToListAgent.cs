using SubhadraSolutions.Utils.Reflection;

namespace SubhadraSolutions.Utils.EventProcessing.Agents;

public class ToListAgent(IEventAggregator eventAggregator) : AbstractAgent(eventAggregator)
{
    protected override void OnEvent(object sender, IEvent e)
    {
        if (e.PayloadObject == null)
        {
            return;
        }

        var list = ReflectionHelper.BuildListIfEnumerableAndNotListOrReturnSame(e.PayloadObject);
        var evt = EventProcessingHelper.BuildGenericEvent(Topic, list, e.Id);
        eventAggregator.PublishEvent(evt);
    }
}