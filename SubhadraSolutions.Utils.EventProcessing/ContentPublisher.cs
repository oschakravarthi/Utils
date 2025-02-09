using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.Data;

namespace SubhadraSolutions.Utils.EventProcessing;

public class ContentPublisher(IEventAggregator eventAggregator) : AbstractEventPublisher(eventAggregator), IRunnable
{
    public IDataSource<object> DataSource { get; set; }
    public object Input { get; set; }

    public void Run()
    {
        PublishEvents();
    }

    protected override void PublishEvents()
    {
        var content = DataSource.GetData();
        var evt = EventProcessingHelper.BuildGenericEvent(Topic, content);
        eventAggregator.PublishEvent(evt);
    }
}