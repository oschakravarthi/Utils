using SubhadraSolutions.Utils.Execution;

namespace SubhadraSolutions.Utils.EventProcessing;

public abstract class AbstractEventingComponent(IEventAggregator eventAggregator) : AbstractComponent
{
    protected readonly IEventAggregator eventAggregator = eventAggregator;
}