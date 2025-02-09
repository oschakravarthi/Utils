using SubhadraSolutions.Utils.EventProcessing.Agents;
using System.Collections;

namespace SubhadraSolutions.Utils.EventProcessing;

public sealed class ForEachAgent(IEventAggregator eventAggregator) : AbstractAgent(eventAggregator)
{
    protected override void OnEvent(object sender, IEvent e)
    {
        var payload = e.PayloadObject;
        if (payload == null)
        {
            return;
        }

        if (payload is not string && payload is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                var itemEvent = EventProcessingHelper.BuildGenericEvent(Topic, item, e.Id);
                eventAggregator.PublishEvent(itemEvent);
            }
        }
    }
}